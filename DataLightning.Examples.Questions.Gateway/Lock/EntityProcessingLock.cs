using org.apache.zookeeper;
using System.Linq;
using System.Threading.Tasks;

namespace DataLightning.Examples.Questions.Gateway.Lock
{
    public class EntityProcessingLock
    {
        private readonly ZooKeeper _zk;

        public EntityProcessingLock(string connectionString)
        {
            _zk = new ZooKeeper(connectionString, 3000, new ZooKeeperWatcher());
        }

        public async Task<bool> LockAsync<T>(int entityId)
        {
            //await _zk.createAsync(
            //    $"/EntityProcessingLock",
            //    null,
            //    ZooDefs.Ids.OPEN_ACL_UNSAFE,
            //    CreateMode.PERSISTENT);

            //await _zk.createAsync(
            //    $"/EntityProcessingLock/{typeof(T).Name}",
            //    null,
            //    ZooDefs.Ids.OPEN_ACL_UNSAFE,
            //    CreateMode.PERSISTENT);

            var lockPath = await _zk.createAsync(
                $"/EntityProcessingLock/{typeof(T).Name}/ID{entityId}",
                null,
                ZooDefs.Ids.READ_ACL_UNSAFE,
                CreateMode.EPHEMERAL_SEQUENTIAL);

            var basePath = $"/EntityProcessingLock/{typeof(T).Name}";
            var nodes = await _zk.getChildrenAsync(basePath, new ZooKeeperWatcher());

            var lockOwner = nodes.Children.OrderBy(c => c).First();

            return lockPath == $"{basePath}/{lockOwner}";
        }
    }
}