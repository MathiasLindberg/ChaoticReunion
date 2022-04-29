using System;
using System.IO;
using UnityEngine;
using LEGO.Logger;

namespace LEGODeviceUnitySDK
{
    internal class InterrogationCacheManager
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(InterrogationCacheManager));

        public static readonly InterrogationCacheManager Instance = new InterrogationCacheManager();

        private static string CacheDir = Path.Combine(Path.Combine(Application.temporaryCachePath, "LEGODeviceSDK"), "Services");

        public ServiceInfoCache CacheForService(IOType type, LEGORevision hwRev, LEGORevision fwRev) {
            return CacheForService(new AttachedIOIdentificationKey(type, firmwareRevision:fwRev, hardwareRevision:hwRev));
        }

        private ServiceInfoCache CacheForService(AttachedIOIdentificationKey key) {
            var filePath = Path.Combine(CacheDir, key.getIdentifier());
            // TODO: Share instances between services; don't read from file each time.
            var res = new ServiceInfoCache(key, filePath);
            res.Initialize();
            return res;
        }

        public void ClearCache()
        {
            try {
                logger.Info("Clearing cache directory "+CacheDir+" ...");
                Directory.Delete(CacheDir, true);
            } catch (FileNotFoundException) {
                // Ignore
            } catch (DirectoryNotFoundException) {
                // Ignore
            } catch (Exception e) {
                logger.Error("Error when clearing cache: "+e);
            }
        }

        internal void EnsureCacheDirectoryExists() {
            if (! Directory.Exists(CacheDir)) {
                var parent = Directory.GetParent(CacheDir);
                if (! parent.Exists) {
                    parent.Create();
                }
                Directory.CreateDirectory(CacheDir);
            }
        }
    }

    internal abstract class CacheableMetadata {
        private static readonly ILog logger = LogManager.GetLogger(typeof(CacheableMetadata));

        private readonly string filePath;
        private bool contentChanged = false;

        public abstract void PopulateFromFileData(string data);
        public abstract string ToFileData();

        protected void MarkAsChanged() {
            contentChanged = true;
        }

        public CacheableMetadata(string filePath) {
            if (filePath == null)
                throw new ArgumentException("filePath may not be null");
            this.filePath = filePath;
        }

        internal void Initialize() {
            string fileData;
            try {
                fileData  = File.ReadAllText(filePath);
            } catch (IOException e) {
                if (!(e is FileNotFoundException || e is DirectoryNotFoundException))
                    logger.Error("Reading cache file "+filePath+" failed: "+e);
                fileData = null;
            } catch (Exception e) {
                // IsolatedStorageException is a wart in that it is not an IOException. We'll treat it as a FileNotFoundException.
                if (e is IOException || e is System.IO.IsolatedStorage.IsolatedStorageException)
                {
                    if (!(e is FileNotFoundException ||
                        e is DirectoryNotFoundException ||
                        e is System.IO.IsolatedStorage.IsolatedStorageException))
                    {
                        // Not just a missing file.
                        logger.Error("Reading cache file "+filePath+" failed: "+e);
                    }
                } else {
                    // This was unexpected.
                    logger.Error("Reading cache file "+filePath+" failed in an obscure way: "+e);
                }
                fileData = null;
            }
            PopulateFromFileData(fileData);
        }

        public void CompletedPopulating() {
            if (contentChanged) {
                WriteCacheFile();
            }
        }

        private void WriteCacheFile() {
            logger.Debug("Writing cache file "+filePath);
            try {
                InterrogationCacheManager.Instance.EnsureCacheDirectoryExists();

                var fileData = ToFileData();
                File.WriteAllText(filePath, fileData);
            } catch (IOException e) {
                logger.Error("Writing cache file "+filePath+" failed: "+e);
            }
        }
    }
}

