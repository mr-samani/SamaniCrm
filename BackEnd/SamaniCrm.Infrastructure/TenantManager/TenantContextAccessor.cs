
using System.Threading;

namespace SamaniCrm.Infrastructure.TenantManager;

public interface ITenantContextAccessor
{
    TenantContext? Current { get; set; }
}
public sealed class TenantContextAccessor : ITenantContextAccessor
{
    private static readonly AsyncLocal<TenantContextHolder?> _current = new();

    public TenantContext? Current
    {
        get => _current.Value?.Context;
        set
        {
            var holder = _current.Value;

            if (holder is not null)
            {
                holder.Context = value;
                return;
            }

            if (value is not null)
            {
                _current.Value = new TenantContextHolder(value);
            }
        }
    }

    private sealed class TenantContextHolder
    {
        public TenantContextHolder(TenantContext context)
        {
            Context = context;
        }

        public TenantContext? Context { get; set; }
    }
}
