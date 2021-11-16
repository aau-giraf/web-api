using System;
using System.Collections.Generic;
using GirafRest.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
namespace GirafRest.Test.RepositoryMocks
 {
    public class MockedUserManager : Mock<UserManager<GirafUser>>
         {
             public MockedUserManager()
                 : this(new Mock<IUserStore<GirafUser>>())
             { }

             public MockedUserManager(Mock<IUserStore<GirafUser>> userStore)
                 : this(
                     userStore,
                     new Mock<IOptions<IdentityOptions>>(),
                     new Mock<IPasswordHasher<GirafUser>>(),
                     new Mock<IEnumerable<IUserValidator<GirafUser>>>(),
                     new Mock<IEnumerable<IPasswordValidator<GirafUser>>>(),
                     new Mock<ILookupNormalizer>(),
                     new Mock<IdentityErrorDescriber>(),
                     new Mock<IServiceProvider>(),
                     new Mock<ILogger<UserManager<GirafUser>>>()
                 )
             { }

             public MockedUserManager(
                     Mock<IUserStore<GirafUser>> userStore,
                     Mock<IOptions<IdentityOptions>> options,
                     Mock<IPasswordHasher<GirafUser>> passwordHasher,
                     Mock<IEnumerable<IUserValidator<GirafUser>>> userValidators,
                     Mock<IEnumerable<IPasswordValidator<GirafUser>>> passwordValidators,
                     Mock<ILookupNormalizer> lookupNormalizers,
                     Mock<IdentityErrorDescriber> identityErrorDescriber,
                     Mock<IServiceProvider> serviceProvider,
                     Mock<ILogger<UserManager<GirafUser>>> logger)
                 : base(
                     userStore.Object,
                     options.Object,
                     passwordHasher.Object,
                     null,
                     null,
                     lookupNormalizers.Object,
                     identityErrorDescriber.Object,
                     serviceProvider.Object,
                     logger.Object
                 )
             {
                 UserStore = userStore;
                 Options = options;
                 PasswordHasher = passwordHasher;
                 UserValidators = userValidators;
                 PasswordValidators = passwordValidators;
                 LookupNormalizers = lookupNormalizers;
                 IdentityErrorDescriber = identityErrorDescriber;
                 ServiceProvider = serviceProvider;
                 Logger = logger;
             }

             public Mock<IUserStore<GirafUser>> UserStore { get; }
             public Mock<IOptions<IdentityOptions>> Options { get; }
             public Mock<IPasswordHasher<GirafUser>> PasswordHasher { get; }
             public Mock<IEnumerable<IUserValidator<GirafUser>>> UserValidators { get; }
             public Mock<IEnumerable<IPasswordValidator<GirafUser>>> PasswordValidators { get; }
             public Mock<ILookupNormalizer> LookupNormalizers { get; } 
             public Mock<IdentityErrorDescriber> IdentityErrorDescriber { get; }
             public Mock<IServiceProvider> ServiceProvider { get; }
             public Mock<ILogger<UserManager<GirafUser>>> Logger { get; }
         }
 } 