using System;
using System.Collections.Generic;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
namespace GirafRest.Test.Mocks
{
    public class MockedRoleManager : RoleManager<GirafRole>
    {
        public MockedRoleManager() : this(new Mock<IRoleStore<GirafRole>>())
        {
        }

        public MockedRoleManager(Mock<IRoleStore<GirafRole>> store) : this(store,
            new Mock<IEnumerable<IRoleValidator<GirafRole>>>(), new Mock<ILookupNormalizer>(),
            new Mock<IdentityErrorDescriber>(), new Mock<ILogger<RoleManager<GirafRole>>>())
        {
        }

        private MockedRoleManager(Mock<IRoleStore<GirafRole>> store,
            Mock<IEnumerable<IRoleValidator<GirafRole>>> rolevalidators, Mock<ILookupNormalizer> keyNormalizer,
            Mock<IdentityErrorDescriber> errors, Mock<ILogger<RoleManager<GirafRole>>>logger) : base(
            store.Object,
            rolevalidators.Object,
            keyNormalizer.Object,
            errors.Object,
            logger.Object)
        {
            Store = store;
            RoleValidators = rolevalidators;
            KeyNormalizor = keyNormalizer;
            Errors = errors;
            Logger = logger;

        }
        
        public Mock<IRoleStore<GirafRole>> Store { get; }
        public Mock<IEnumerable<IRoleValidator<GirafRole>>> RoleValidators { get; }
        public Mock<ILookupNormalizer> KeyNormalizor { get; }
        public Mock<IdentityErrorDescriber> Errors { get; }
        public Mock<ILogger<RoleManager<GirafRole>>> Logger { get; }
    }
}