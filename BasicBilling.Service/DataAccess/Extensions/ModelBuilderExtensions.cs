using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BasicBilling.Service.DataAccess.Extensions
{
    internal static class ModelBuilderExtensions
    {
        public static ModelBuilder Build(
            this ModelBuilder modelBuilder, Assembly assembly)
        {
            var applyGenericMethod = typeof(ModelBuilder)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Single(m => m.Name == nameof(ModelBuilder.ApplyConfiguration)
                             && m.GetParameters().Count() == 1
                             && m.GetParameters().Single().ParameterType.GetGenericTypeDefinition() ==
                             typeof(IEntityTypeConfiguration<>));
            foreach (var type in assembly.GetTypes()
                .Where(c => c.IsClass && !c.IsAbstract && !c.ContainsGenericParameters))

                foreach (var iface in type.GetInterfaces())
                    if (iface.IsConstructedGenericType &&
                        iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                    {
                        var applyConcreteMethod = applyGenericMethod.MakeGenericMethod(iface.GenericTypeArguments[0]);
                        applyConcreteMethod.Invoke(modelBuilder, new[] { Activator.CreateInstance(type) });
                        break;
                    }

            return modelBuilder;
        }
    }
}
