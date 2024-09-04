using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PizzaDelivery.Interfaces;

namespace PizzaDelivery.Extensions
{
    public static class ChangeTrackerExtensions
    {
        public static void SetAuditProperties(this ChangeTracker changeTracker, IUsuarioService usuarioService)
        {
            changeTracker.DetectChanges();

            IEnumerable<EntityEntry> auditableEntities = changeTracker.Entries()
                .Where(t => t.Entity is IAuditable &&
                    (t.State == EntityState.Added || t.State == EntityState.Modified));

            if (auditableEntities.Any())
            {
                //DateTimeOffset timestamp = DateTimeOffset.UtcNow;
                DateTime timestamp = DateTime.Now;

                var currentUserId = usuarioService.GetCurrentUser().currentUserId;

                foreach (EntityEntry entry in auditableEntities)
                {

                    // Evitar gravar auditoria de Alteração, caso seja soft delete.
                    if ((entry.Metadata.FindProperty("IsExcluido") != null)
                        && ((bool)entry.Property("IsExcluido").CurrentValue))
                        continue;

                    IAuditable entity = (IAuditable)entry.Entity;

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            // A data de inclusão é readonly e é definida no DB
                            //entity.DataInclusao = timestamp;

                            // Se for criação de usuário admin, mantem id atribuído no Inicializador
                            entity.UsuarioIdInclusao ??= currentUserId;
                            break;
                        case EntityState.Modified:
                            entity.DataAlteracao = timestamp;
                            entity.UsuarioIdAlteracao = currentUserId;
                            break;
                    }
                }
            }

            IEnumerable<EntityEntry> softDeletableEntities = changeTracker
                    .Entries()
                    .Where(t => t.Entity is ISoftDeletable && t.State == EntityState.Deleted);

            if (softDeletableEntities.Any())
            {
                //DateTimeOffset timestamp = DateTimeOffset.UtcNow;
                DateTime timestamp = DateTime.Now;

                var currentUserId = usuarioService.GetCurrentUser().currentUserId;

                foreach (EntityEntry entry in softDeletableEntities)
                {
                    ISoftDeletable entity = (ISoftDeletable)entry.Entity;
                    entity.DataExclusao = timestamp;
                    entity.UsuarioIdExclusao = currentUserId;
                    entity.IsExcluido = true;
                    entry.State = EntityState.Modified;
                }
            }
        }
    }
}