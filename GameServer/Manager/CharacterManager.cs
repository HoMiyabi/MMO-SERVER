using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
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

        public CharacterManager()
        {
            saveTimer = new Timer(Save, null, 0, 2);
        }

        private void Save(object _)
        {
            var repository = Db.fsql.GetRepository<DbCharacter>();
            foreach (var character in idToCharacter.Values)
            {
                Log.Information($"保存角色 {character.dbCharacter.X} {character.dbCharacter.Y} {character.dbCharacter.Z}");
                repository.UpdateAsync(character.dbCharacter);
            }
        }

        public Character CreateCharacter(DbCharacter dbCharacter)
        {
            var character = new Character(dbCharacter);
            if (!idToCharacter.TryAdd(character.characterId, character))
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
            EntityManager.Instance.RemoveEntity(character.dbCharacter.SpaceId, character);
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