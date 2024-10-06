using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FreeSql;
using GameServer.Database;
using GameServer.Model;
using Kirara;
using Serilog;

namespace GameServer.Manager
{
    public class CharacterManager : Singleton<CharacterManager>
    {
        // Character Id
        private readonly ConcurrentDictionary<int, Character> idToCharacter = new();
        private Timer saveTimer;

        private IBaseRepository<DbCharacter> dbCharacterRepo = Db.fsql.GetRepository<DbCharacter>();

        public CharacterManager()
        {
            saveTimer = new Timer(Save, null, 0, 2000);
        }

        private void Save(object _)
        {
            Db.fsql.Update<DbCharacter>()
                .SetSource(idToCharacter.Values.Select(character => character.DbCharacter))
                .ExecuteAffrows();
        }

        public Character CreateCharacter(DbCharacter dbCharacter)
        {
            var character = new Character(dbCharacter);
            if (!idToCharacter.TryAdd(character.id, character))
            {
                Log.Warning($"不能添加角色到字典 {dbCharacter.Id.NameValue()}");
            }
            EntityManager.Instance.AddEntity(dbCharacter.SpaceId, character);
            return character;
        }

        public void RemoveCharacter(int characterId)
        {
            if (!idToCharacter.TryRemove(characterId, out var character))
            {
                Log.Warning($"字典找不到角色 {characterId.NameValue()}");
            }
            EntityManager.Instance.RemoveEntity(character.spaceId, character);
        }

        public void ClearCharacter()
        {
            idToCharacter.Clear();
        }

        public Character GetCharacter(int characterId)
        {
            return idToCharacter.GetValueOrDefault(characterId, null);
        }

        public void Close()
        {
            saveTimer.Dispose();
        }
    }
}