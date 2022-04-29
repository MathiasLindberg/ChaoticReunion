using System;
using System.Collections.Generic;
using System.Diagnostics;
using LEGO.Logger;

namespace LEGODeviceUnitySDK
{
    /*! \brief
     * This class is provides running statistics on the transfer of data via the OAD processes to a hub
     * It is intended to be used for adaptive throttling of data transfer to the hub
     * The general mechanism is to call StartSession, then call appropriate methods indication resend requests from the
     * hub, blocks being sent etc.
     * Stats can be extracted either by reading running properties or (later) by subscribing to statistical information.
     */
    public class OADTransferStats
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(OADTransferStats));
        public event Action<float> OnBytePerMsRateUpdated;
        public event Action<float> OnResendsPerBlockSent;
        
        private Stopwatch stopWatch;
        private uint dataSizeBytes;
        private uint dataSentBytes;
        private uint dataResentBytes;
        private class BlockSentEntry
        {
            public long elapsedMs;
            public uint blockSizeBytes;
            public uint blockNumber; 
        }
        private List<BlockSentEntry> blockSentEntries;
        private Queue<BlockSentEntry> blockSentWindowEntries;
        private uint blocksSentCount;
        
        private class RepeatRequestEntry
        {
            public long elapsedMs;
            public uint blockNumber;
        }
        private List<RepeatRequestEntry> repeatRequestBlockEntries;
        private Queue<BlockSentEntry> repeatRequestBlockWindowEntries;
        private uint repeatCount;

        public enum StatsMode
        {
            FullCapture,
            WindowCaptureOnly // should we add "both"?
        };
        private StatsMode currentStatsMode;
        private uint byteRateWindowSize;
        private uint repeatRateWindowSize;
        
        public void StartSession(uint currentBlockSize, uint dataSizeBytes, uint byteRateWindowSize=10, uint repeatRateWindowSize = 1000, StatsMode statsMode=StatsMode.FullCapture  )
        {
            try
            {
                Reset();
                
                currentStatsMode = statsMode;
                this.byteRateWindowSize = (byteRateWindowSize > 1) ? byteRateWindowSize : 2; // never less than 2
                this.repeatRateWindowSize = (repeatRateWindowSize > 1) ? repeatRateWindowSize : 5; // never less than 5
                
                if (currentStatsMode == StatsMode.FullCapture)
                {
                    blockSentEntries = new List<BlockSentEntry>((int)(currentBlockSize/dataSizeBytes));
                    repeatRequestBlockEntries = new List<RepeatRequestEntry>(((int) currentBlockSize / (int) (dataSizeBytes * 4)));
                }

                blockSentWindowEntries = new Queue<BlockSentEntry>((int)this.byteRateWindowSize);
                repeatRequestBlockWindowEntries = new Queue<BlockSentEntry>((int)this.repeatRateWindowSize);
                
                this.dataSizeBytes  = dataSizeBytes;

            }
            catch (Exception e)
            {
                logger.Error("Fatal start session parameters - possibly divide by zero? : " + e.ToString());
            }
        }

        private void Reset()
        {
            dataSentBytes  = 0;
            dataResentBytes = 0;
            blocksSentCount = 0;
            repeatCount = 0;
            lastEntryTimeStampMs = 0;
            lastRepeatEntryTimestampMs = 0;
            lastResendRatePerBlockSent = -1;
            lastCurrentTransferRateBytePerMs = 0;
            stopWatch = new Stopwatch();
        }
        
        private long lastRepeatEntryTimestampMs;
        public void RepeatRequest(uint blockNumber, uint blockSizeBytes)
        {
            if (stopWatch == null)
            {   // not reset/setup
                return;
            } 
            
            long currentTimeStampMs = stopWatch.ElapsedMilliseconds;
            long entryDeltaTimeMs = currentTimeStampMs - lastRepeatEntryTimestampMs; 
            if (currentStatsMode == StatsMode.FullCapture)
            {
                repeatRequestBlockEntries?.Add(new RepeatRequestEntry() { elapsedMs = currentTimeStampMs, blockNumber = blockNumber });
            }
            
            repeatRequestBlockWindowEntries?.Enqueue(new BlockSentEntry()
            {
                elapsedMs = (blockSentWindowEntries?.Count == 0 ? 0 : entryDeltaTimeMs), 
                blockNumber = blockNumber, blockSizeBytes = blockSizeBytes
            } );
            
            while (   repeatRequestBlockWindowEntries?.Count > 0 
                   && repeatRequestBlockWindowEntries?.Peek().blockNumber <= blocksSentCount - repeatRateWindowSize)
            {
                repeatRequestBlockWindowEntries?.Dequeue();
            }
            
            dataResentBytes += blockSizeBytes;
            repeatCount++;
            lastRepeatEntryTimestampMs = currentTimeStampMs;

            string dbg = $"*DRAT* bn={blockNumber} bs={blocksSentCount} rpWinCnt={repeatRequestBlockWindowEntries?.Count} entryDt={entryDeltaTimeMs:F0}";
            logger.Debug(dbg);
        }

        private long lastEntryTimeStampMs;
        public void SentBlock(uint blockNumber, uint blockSizeBytes)
        {
            if(blocksSentCount == 0)
            {
                stopWatch?.Start();
            }

            long currentTimeStampMs = stopWatch.ElapsedMilliseconds;
            long entryDeltaTimeMs = currentTimeStampMs - lastEntryTimeStampMs; 
            blockSentWindowEntries?.Enqueue(new BlockSentEntry()
            {
                elapsedMs = (blockSentWindowEntries?.Count == 0 ? 0 : entryDeltaTimeMs), 
                blockNumber = blockNumber, blockSizeBytes = blockSizeBytes
            } );
            
            if (blockSentWindowEntries?.Count > byteRateWindowSize)
            {
                blockSentWindowEntries?.Dequeue(); // discard the oldest entry 
            }

            if (currentStatsMode == StatsMode.FullCapture)
            {
                blockSentEntries?.Add(new BlockSentEntry()
                {
                    elapsedMs = (blockSentEntries?.Count == 0 ? 0 : currentTimeStampMs), 
                    blockNumber = blockNumber, blockSizeBytes = blockSizeBytes
                });
            }

            dataSentBytes += blockSizeBytes;
            blocksSentCount++;
            
            lastEntryTimeStampMs = currentTimeStampMs;
            
            if (blocksSentCount % byteRateWindowSize == 0)
            {
                CurrentTransferRateBytePerMs(); // causes an event if difference from last > 10%
            }
            
            CurrentResendRatePerBlockSent(); // causes an event if difference from last > 10%
        }

        public void EndSession()
        {
            if (stopWatch == null)
            {   // was never setup - return
                return;
            }
            
            stopWatch.Stop();

            // temp stuff below
            if (currentStatsMode == StatsMode.FullCapture)
            {
                var lastLogLevel = LogManager.RootLevel;
                LogManager.RootLevel = LogLevel.DEBUG;
                logger.Debug("***OAD Transfer Stats Summary*** \n" + ToString("SUMMARY"));
                logger.Debug("***OAD Transfer Stats Tables*** \n" + ToString("ALL"));
                LogManager.RootLevel = lastLogLevel;
            }
        }

        private float lastCurrentTransferRateBytePerMs;
        public float CurrentTransferRateBytePerMs()
        {
            if (blockSentWindowEntries?.Count <= 1)
            {   // not enough data points yet
                return 0;
            }
            
            uint bytesSentDuringWindow = 0;
            long msUsedDuringWindow = 0;
            foreach (BlockSentEntry blockSentEntry in blockSentWindowEntries)
            {
                msUsedDuringWindow += blockSentEntry.elapsedMs;
                bytesSentDuringWindow += blockSentEntry.blockSizeBytes;
            }

            if (msUsedDuringWindow == 0)
            {
                return 0;
            }
            
            float bytesPerMs = (float)bytesSentDuringWindow / (float)msUsedDuringWindow;

            float differenceFromLast = Math.Abs(bytesPerMs - lastCurrentTransferRateBytePerMs);
            if (OnBytePerMsRateUpdated != null && differenceFromLast > (lastCurrentTransferRateBytePerMs * 0.10f))
            {
                OnBytePerMsRateUpdated(bytesPerMs);
            }

            lastCurrentTransferRateBytePerMs = bytesPerMs;
            return bytesPerMs;
        }

        private float lastResendRatePerBlockSent; 
        private float CurrentResendRatePerBlockSent()
        {
            float rate = 0;
            if (   blocksSentCount >= 0
                && repeatRateWindowSize >= 0)
            {
                if (blocksSentCount < repeatRateWindowSize)
                {
                    rate = (float) repeatRequestBlockWindowEntries?.Count / (float) blocksSentCount;
                }
                else
                {
                    rate = (float) repeatRequestBlockWindowEntries?.Count / (float) repeatRateWindowSize;
                }
            }
            
            float differenceFromLast = Math.Abs(rate - lastResendRatePerBlockSent);
            if (OnResendsPerBlockSent != null && differenceFromLast > (lastResendRatePerBlockSent * 0.10f))
            {
                string dbg = $"*DRAT* rate={rate} bs={blocksSentCount} rpWinCnt={repeatRequestBlockWindowEntries?.Count} diff={differenceFromLast:F2}";
                logger.Debug(dbg);
                OnResendsPerBlockSent(rate);
            }

            lastResendRatePerBlockSent = rate;
            return rate;
        }
        
        public float OverallTransferRateBytePerMs()
        {
            if (   blocksSentCount > 0 
                && blockSentWindowEntries?.Count > 0
                && blockSentWindowEntries?.Peek().elapsedMs > 0)
            {
                return (float) (dataSentBytes - dataResentBytes) / blockSentWindowEntries.Peek().elapsedMs;
            }
            return 0;
        }
        
        public float OverallRepeatRatePerBlockSent()
        {
            if (blocksSentCount > 0)
            {
                return (float) repeatCount / (float)blocksSentCount;
            }
            return 0;
        }

        public override string ToString()
        {
            return ToString("");
        }
        
        public string ToString(string formatString="")
        {
            switch (formatString.ToLower())
            {
                case "all":
                    return "**ByteRateTable**\n" + ByteRateTable() + "\n" + "**RepeatRateTable**\n" + RepeatRateTable();
                case "repeatratetable": return RepeatRateTable();
                case "byteratetable":   return ByteRateTable();
                case "summary":
                    return "Bytes Per Ms: " + OverallTransferRateBytePerMs()
                                            + "\n repeat per block sent: " + OverallRepeatRatePerBlockSent()
                                            + "\n";
                default:
                    return base.ToString();
            }
        }

        private string RepeatRateTable()
        {
            if (currentStatsMode != StatsMode.FullCapture)
            {
                return "StatsMode was not fullcapture - no data";
            }
            string repeatStats = ""; // = "***REPEAT STATS blockNumber timeMs\n";
            foreach (var repeat in repeatRequestBlockEntries)
            {
                repeatStats += repeat.blockNumber + " " + repeat.elapsedMs + "\n";
            }
            return repeatStats;
        }
        
        private string ByteRateTable()
        {
            if (currentStatsMode != StatsMode.FullCapture)
            {
                return "StatsMode was not fullcapture - no data";
            }
            string blockStats = ""; //"***BLOCK STATS blockNumber sizeBytes timeMs\n";
            foreach (var blockSent in blockSentEntries)
            {
                blockStats += blockSent.blockNumber + " " + blockSent.blockSizeBytes + " " + blockSent.elapsedMs + "\n";
            }
            return blockStats;
        }
    }
}