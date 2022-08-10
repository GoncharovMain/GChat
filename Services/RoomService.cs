namespace GChat.Services
{
    public class RoomService
    {
        private Dictionary<string, string> _room = new();

        public RoomService() { }

        // Summary:
        //   dictionary online and offline connection id storage service.
        //
        // Parameters:
        //   key: connection id of hub
        //   value: room id
        public string this[string connectionId]
        {
            get
            {
                if (_room.Keys.Contains(connectionId))
                    return _room[connectionId];

                throw new Exception($"User: {connectionId} is not included in the group.");
            }
            set
            {
                if (_room.Keys.Contains(connectionId))
                    _room[connectionId] = value;
                else
                    _room.Add(connectionId, value);
            }
        }

        public List<string> Connectors => _room.Keys.ToList();

        public bool Remove(string connectionId) => _room.Remove(connectionId);
    }
}