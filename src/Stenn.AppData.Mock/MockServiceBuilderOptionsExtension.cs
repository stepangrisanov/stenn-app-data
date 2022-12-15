using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Stenn.AppData.Mock
{
    internal sealed class MockServiceBuilderOptionsExtension<TBaseEntity> : IDbContextOptionsExtension
    {
        private ExtensionInfo<TBaseEntity>? _info;

        public Action<ModelBuilder>? Action { get; }

        public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo<TBaseEntity>(this);

        public MockServiceBuilderOptionsExtension(Action<ModelBuilder> action)
        {
            this.Action = action;
        }

        public void ApplyServices(IServiceCollection services)
        {
        }

        public void Validate(IDbContextOptions options)
        {
        }

        private sealed class ExtensionInfo<T> : DbContextOptionsExtensionInfo
        {
            private string? _logFragment;

            public ExtensionInfo(MockServiceBuilderOptionsExtension<T> extension)
                : base(extension)
            {
            }

            private new MockServiceBuilderOptionsExtension<T> Extension => (MockServiceBuilderOptionsExtension<T>)base.Extension;

            public override bool IsDatabaseProvider => false;

            public override string LogFragment
            {
                get
                {
                    if (_logFragment != null)
                    {
                        return _logFragment;
                    }

                    _logFragment = $"Action method = {Extension.Action}";

                    return _logFragment;
                }
            }

#if NET6_0_OR_GREATER
            /// <inheritdoc />
            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
                return other is ExtensionInfo<T> otherInfo &&
                       otherInfo.GetServiceProviderHashCode() == GetServiceProviderHashCode();
            }

            public override int GetServiceProviderHashCode()
            {
                return 0;
            }
#else
            /// <inheritdoc />
            public override long GetServiceProviderHashCode()
            {
                return 0;
            }
#endif

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                if (debugInfo == null)
                {
                    throw new ArgumentNullException(nameof(debugInfo));
                }

                debugInfo["Action method: "] = Extension.Action?.ToString() ?? "none";
            }
        }
    }
}