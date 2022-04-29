using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using LEGO.Logger;

namespace LEGODeviceUnitySDK
{

    /// <summary>
    /// Tracks the fate of individual service commands.
    /// </summary>
    public class ServiceCommandTracker : IServiceStateMachineListener
    {
        private const int SERVICE_QUEUE_SIZE = 2;

        private static readonly ILog logger = LogManager.GetLogger(typeof(ServiceCommandTracker));

        private readonly ServiceStateMachine stateMachine;
        private readonly Action onReadyForNextCommand;

        public ServiceCommandTracker(Action onReadyForNextCommand)
        {
            this.stateMachine = new ServiceStateMachine(this);
            this.onReadyForNextCommand = onReadyForNextCommand;
        }

        #region Public operations
        public bool HasRoomForMoreQueued { get { return commandCompletionActions.Count < SERVICE_QUEUE_SIZE; } }

        internal void CommandSent(Action<bool> completionAction, ObservablePacketDropFeedback obsFeedback)
        {
            Assert.IsTrue(completionAction != null);
            commandCompletionActions.Enqueue(obsFeedback, completionAction);
        }

        public void FeedbackReceived(byte feedback)
        {
            // REMOVE UNDOCUMENTED BIT FIELDS (ARGH!!!!)
            FeedbackReceived((ServiceStateMachine.InEvent)(feedback & 0x1F));
        }

        public void FeedbackReceived(ServiceStateMachine.InEvent feedback)
        {
            stateMachine.HandleEvent(feedback);
        }

        public void Clear()
        {
            Action<bool> completionAction;
            while (commandCompletionActions.TryDequeue(out completionAction)) {
                completionAction(false);
            }
            stateMachine.Clear();
        }

        #endregion

        #region State machine callbacks
        private readonly CommandCompletionQueue commandCompletionActions = new CommandCompletionQueue();

        public void OnCommandDone(bool isCompleted)
        {
            if (commandCompletionActions.IsEmpty) {
                // The Vision sensor appears to send extra (Idle+Empty+Completed) feedback.
                logger.Info("Non-existent command completed - queue is extra empty.");
                return;
            }
            Assert.IsTrue(!commandCompletionActions.IsEmpty);
            var completionAction = commandCompletionActions.Dequeue();
            completionAction(isCompleted);

            if (HasRoomForMoreQueued)
            {
                if (onReadyForNextCommand != null) onReadyForNextCommand();
            }
        }
        #endregion

    }


    public interface IServiceStateMachineListener
    {
        /// <summary>
        /// The next command in the queues completes or is cancelled.
        /// </summary>
        void OnCommandDone(bool isCompleted);
    }

    public class ServiceStateMachine
    {
        enum State
        {
            Idle,
            BusyEmpty, // One in progress, none in buffer
            BusyFull   // One in progress, one in buffer
        }



        // NOTE: There is an undocumented "motor stall" bit present in this field (and probably others...) 
        //       this bit is bit 5 in the received event flag - removed at reception
        public enum InEvent
        {
            // New states:
            BusyEmpty = 0x01,
            Idle      = 0x08,
            BusyFull  = 0x10,

            // Other flags:
            Completed = 0x02,
            Discarded = 0x04
        }

        private State state = State.Idle;
        private readonly IServiceStateMachineListener listener;

        private static readonly ILog logger = LogManager.GetLogger(typeof(ServiceStateMachine));

        public ServiceStateMachine(IServiceStateMachineListener listener)
        {
            this.listener = listener;
        }

        public bool IsCommandInProgress { get { return state != State.Idle; } }
        public bool IsBufferFull { get { return state == State.BusyFull; } }

        public void Clear()
        {
            state = State.Idle;
        }

        public void HandleEvent(InEvent inEvent)
        {
            this.state = HandleEventCalculatingNewState(inEvent);
        }

        private State HandleEventCalculatingNewState(InEvent inEvent)
        {
            //logger.Debug("Handle new event: " + inEvent + " in state: " + state);

            switch (state)
            {
            case State.Idle:
                switch (inEvent)
                {
                case InEvent.Idle | InEvent.Completed:
                    listener.OnCommandDone(true);
                    return State.Idle;
                case InEvent.Idle | InEvent.Discarded:
                    logger.Info("Non-existent command discarded - hub non-compliance detected : " + inEvent );
                    return State.Idle;
                case InEvent.BusyEmpty:
                    // We trust listener to have checked buffer state and to have started new command in the right place.
                    return State.BusyEmpty;
                }
                break;

            case State.BusyEmpty:
                switch (inEvent)
                {
                case InEvent.Idle | InEvent.Completed:
                    listener.OnCommandDone(true);
                    return State.Idle;
                case InEvent.Idle | InEvent.Discarded:
                    listener.OnCommandDone(false);
                    return State.Idle;
                case InEvent.Idle | InEvent.Completed | InEvent.Discarded:
                    listener.OnCommandDone(false);
                    listener.OnCommandDone(true);
                    return State.Idle;
                case InEvent.BusyEmpty | InEvent.Discarded:
                    listener.OnCommandDone(false);
                    return State.BusyEmpty;
                case InEvent.BusyFull:
                    return State.BusyFull;
                }
                break;
            case State.BusyFull:
                switch (inEvent)
                {
                case InEvent.Idle | InEvent.Completed:
                    listener.OnCommandDone(true);
                    listener.OnCommandDone(true);
                    return State.Idle;
                case InEvent.Idle | InEvent.Discarded:
                    listener.OnCommandDone(false);
                    listener.OnCommandDone(false);
                    return State.Idle;
                case InEvent.Idle | InEvent.Completed | InEvent.Discarded:
                    listener.OnCommandDone(false);
                    listener.OnCommandDone(false);
                    listener.OnCommandDone(true);
                    return State.Idle;
                case InEvent.BusyEmpty | InEvent.Completed:
                    listener.OnCommandDone(true);
                    return State.BusyEmpty;
                case InEvent.BusyEmpty | InEvent.Discarded:
                    listener.OnCommandDone(false);
                    listener.OnCommandDone(false);
                    return State.BusyEmpty;
                }
                break;
            }
            throw new Exception("Unexpected event: "+inEvent+" in state: "+state);
        }

    }


    class ObservablePacketDropFeedback: IPacketDropFeedback {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ObservablePacketDropFeedback));

        private Action onDropped;

        public void Subscribe(Action action) {
            onDropped = action;
        }
        public void Unsubscribe() {
            onDropped = null;
        }

        public void OnDropped()
        {
            if (onDropped != null) {
                onDropped();
            } else {
                logger.Error("Packet OnDropped is lost - no observer!");
            }
        }
    }

    /// For the command queue, we need a data structure which is a queue with the possibility of deleting a specific element.
    class CommandCompletionQueue {
        // OPTIMIZATION opportunity: having a short list of empty Elements, to reduce GC churn.

        private readonly Element ends;
        private int count = 0;

        public CommandCompletionQueue()
        {
            // Create sentinel:
            ends = new Element(this, null, null);
            ends.prev = ends.next = ends; 
        }

        public void Enqueue(ObservablePacketDropFeedback observable, Action<bool> completionAction) {
            var element = new Element(this, observable, completionAction);
            element.InsertAtTail();
        }

        public bool IsEmpty { get { return ends.next == ends; } }
        public int Count { get { return count; } }

        public Action<bool> Dequeue() {
            Action<bool> completionAction;
            if (TryDequeue(out completionAction)) {
                return completionAction;
            }  else {
                throw new Exception("Invalid state: Dequeueing an empty queue.");
            }
        }

        public bool TryDequeue(out Action<bool> result) {
            var head = ends.next;
            if (head == ends) {
                result = null;
                return false;
            }

            head.Remove();
            result = head.completionAction;
            return true;
        }

        private class Element/*: IPacketDropFeedback*/ {
            internal readonly CommandCompletionQueue queue;
            internal Element prev, next;
            internal readonly Action<bool> completionAction;
            private ObservablePacketDropFeedback observable; // Keep on to this! - it is only weakly referred to by others.

            public Element(CommandCompletionQueue queue, ObservablePacketDropFeedback observable, Action<bool> completionAction)
            {
                this.queue = queue;
                this.observable = observable;
                this.completionAction = completionAction;
                if (observable != null) {
                    this.observable.Subscribe(this.OnDropped);
                }
            }
            
            private void OnDropped()
            {
                Remove();
                completionAction(false);
            }

            internal void InsertAtTail() {
                prev = queue.ends.prev;
                next = queue.ends;
                prev.next = this;
                next.prev = this;
            }

            internal void Remove()
            {
                // Remove self from chain:
                prev.next = next;
                next.prev = prev;
                next = prev = null;
                queue.count--;

                observable.Unsubscribe();
            }
        }

    }
}

