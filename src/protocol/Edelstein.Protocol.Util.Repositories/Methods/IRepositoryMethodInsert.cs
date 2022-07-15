﻿namespace Edelstein.Protocol.Util.Repositories.Methods;

public interface IRepositoryMethodInsert<in TKey, TEntry> where TEntry : IIdentifiable<TKey>
{
    Task<TEntry> Insert(TEntry entry);
}