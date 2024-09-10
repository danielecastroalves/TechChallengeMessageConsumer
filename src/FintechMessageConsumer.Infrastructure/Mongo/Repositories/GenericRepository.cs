using System.Linq.Expressions;
using FintechMessageConsumer.Application.Common.Repositories;
using FintechMessageConsumer.Domain.Entities;
using FintechMessageConsumer.Infrastructure.Mongo.Contexts.Interfaces;
using MongoDB.Driver;

namespace FintechMessageConsumer.Infrastructure.Mongo.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly IMongoContext _context;

        public GenericRepository(IMongoContext context) => _context = context;

        public virtual async Task<Guid> AddAsync
        (
            TEntity entity,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.SetDataInsercao();

            await _context.GetCollection<TEntity>()
                .InsertOneAsync(entity, cancellationToken: cancellationToken);

            return entity.Id;
        }

        public virtual async Task<TEntity> GetByFilterAsync
        (
            Expression<Func<TEntity, bool>> filter,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryResut = await _context.GetCollection<TEntity>()
                .FindAsync(filter, cancellationToken: cancellationToken);

            return await queryResut.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<TEntity> GetByIdAsync
        (
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryResut = await _context.GetCollection<TEntity>()
                .FindAsync(f => f.Id == id, cancellationToken: cancellationToken);

            return await queryResut.FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> GetListByFilterAsync
        (
            Expression<Func<TEntity, bool>> filter,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _context.GetCollection<TEntity>()
                .FindAsync(filter, cancellationToken: cancellationToken);

            return await result.ToListAsync(cancellationToken);
        }

        public virtual async Task UpdateAsync
        (
            Expression<Func<TEntity, bool>> filter,
            TEntity entity,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            entity.SetDataAtualizacao();

            await _context.GetCollection<TEntity>()
                .ReplaceOneAsync(filter,
                                 entity,
                                 new ReplaceOptions { IsUpsert = true },
                                 cancellationToken: cancellationToken);
        }

        public virtual async Task<bool> DeleteByIdAsync
        (
            Guid id,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryResut = await _context.GetCollection<TEntity>()
                .DeleteOneAsync(f => f.Id == id, cancellationToken: cancellationToken);

            return queryResut.IsAcknowledged;
        }
    }
}
