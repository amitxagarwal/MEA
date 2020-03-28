using AutoFixture;
using AutoFixture.Idioms;
using FluentAssertions;
using FluentAssertions.Types;
using Kmd.Momentum.Mea.Api;
using Kmd.Momentum.Mea.Common.DatabaseStore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Kmd.Momentum.Mea.Tests.Database
{
    public class DocumentSerialisationTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DocumentSerialisationTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public static IEnumerable<object[]> GetConcreteDocumentTypes()
        {
            var concreteDocumentTypes = Startup
                .MeaAssemblyDiscoverer
                .DiscoverConcreteDocumentTypes()
                .ToArray();

            foreach (var documentType in concreteDocumentTypes)
            {
                yield return new object[] { documentType };
            }
        }

        static void AssertTypeHasValidIdProperty(Type documentType)
        {
            var expectedKeyType = typeof(Guid);

            foreach (var intf in documentType.GetInterfaces())
            {
                if (intf.IsGenericType && intf.GetGenericTypeDefinition() == typeof(IDocument<>))
                {
                    expectedKeyType = intf.GetGenericArguments()[0];
                }
            }

            var idProperty = documentType.GetProperty(nameof(IDocument.Id));

            var assertions = idProperty.Should()
                .NotBeNull()
                .And.BeReadable()
                .And.NotBePubliclyWritable()
                .And.Return(expectedKeyType)
                .And.BeDecoratedWith<Newtonsoft.Json.JsonPropertyAttribute>(a => a.PropertyName == "id");
        }

        [Theory]
        [MemberData(nameof(GetConcreteDocumentTypes))]
        public void HasPublicIdPropertyWithCorrectConventions(Type documentType)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            AssertTypeHasValidIdProperty(documentType);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        [Theory]
        [MemberData(nameof(GetConcreteDocumentTypes))]
        public void AllPublicPropertiesAreReadOnly(Type documentType)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            var allPublicProperties = documentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
#pragma warning restore CA1062 // Validate arguments of public methods

            foreach (var property in allPublicProperties)
            {
                property.Should().BeReadable();

                var setter = property.GetSetMethod();
                if (setter != null)
                {
                    setter.IsPrivate.Should().Be(true);
                }
            }
        }

        /// <summary>
        /// This test demonstrates how the <see cref="AssertTypeHasValidIdProperty"/>
        /// works, and what kind of exception+message it will throw when an <see cref="IDocument"/>
        /// has a publicly writeable Id property.
        /// </summary>
        [Fact]
        public void AssertionThrowsIfConcreteDocumentTypeIdIsWriteable()
        {
            Action act = () => AssertTypeHasValidIdProperty(typeof(ClassWithPublicIdSetter));
            var expectedMessage = "Expected property Id not to have a public setter.";
            act.Should().ThrowExactly<Xunit.Sdk.XunitException>().WithMessage(expectedMessage);
        }

        [Fact]
        public void DocumentMappableTestDiscoversAllTypes()
        {
            var assemblies = new HashSet<Assembly> { this.GetType().Assembly };
            var seenTypes = new HashSet<Type>();

            CheckForDocumentMappableAttribute(typeof(ClassWithDifferentMappableName), assemblies, seenTypes);

            var expectedTypes = new HashSet<Type>
            {
                typeof(Guid),
                typeof(string),
                typeof(ClassWithDifferentMappableName),
                typeof(ChildClassWithDifferentMappableName),
                typeof(List<ChildClassWithDifferentMappableName>),
                typeof(IReadOnlyList<ChildClassWithDifferentMappableName>),
                typeof(IChildClassWithDifferentMappableName),
                typeof(List<IChildClassWithDifferentMappableName>),
                typeof(ChildClassWithDifferentMappableNameBase),
                typeof(IList<ChildClassWithDifferentMappableNameBase>),
                typeof(IReadOnlyDictionary<string, ChildClassWithDifferentMappableNameBase>),
                typeof(ChildClassWithDifferentMappableNameBase[])
            };

            seenTypes.Should().BeEquivalentTo(expectedTypes);
        }

        [Theory]
        [MemberData(nameof(GetConcreteDocumentTypes))]
        public void AllDocumentsAndReferencedClassesHaveDocumentMappableAttribute(Type documentType)
        {
            var assemblies = new HashSet<Assembly>(Startup.MeaAssemblyDiscoverer.Assemblies);
            var seenTypes = new HashSet<Type>();

#pragma warning disable CA1062 // Validate arguments of public methods
            CheckForDocumentMappableAttribute(documentType, assemblies, seenTypes);
#pragma warning restore CA1062 // Validate arguments of public methods
        }

        private void CheckForDocumentMappableAttribute(Type type, HashSet<Assembly> assemblies, HashSet<Type> seenTypes)
        {
            if (!seenTypes.Add(type)) return;

            if (type.IsEnum) return;

            if (type.IsArray)
            {
                CheckForDocumentMappableAttribute(type.GetElementType(), assemblies, seenTypes);
            }
            else if (type.IsGenericType)
            {
                foreach (var genericType in type.GetGenericArguments())
                {
                    CheckForDocumentMappableAttribute(genericType, assemblies, seenTypes);
                }
            }
            else if (type.IsInterface)
            {
                foreach (var ass in assemblies)
                {
                    foreach (var implementingType in ass.GetTypes())
                    {
                        if (implementingType.IsClass && type.IsAssignableFrom(implementingType))
                        {
                            CheckForDocumentMappableAttribute(implementingType, assemblies, seenTypes);
                        }
                    }
                }
            }
            else if (type.IsAbstract)
            {
                foreach (var ass in assemblies)
                {
                    foreach (var implementingType in ass.GetTypes())
                    {
                        if (implementingType.IsClass && type.IsAssignableFrom(implementingType))
                        {
                            CheckForDocumentMappableAttribute(implementingType, assemblies, seenTypes);
                        }
                    }
                }
            }
            else if (assemblies.Contains(type.Assembly))
            {
                var attr = type.GetCustomAttribute<DocumentMappableAttribute>();
                attr.Should().NotBeNull($"{type} must have the {nameof(DocumentMappableAttribute)}");

                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    CheckForDocumentMappableAttribute(prop.PropertyType, assemblies, seenTypes);
                }

                foreach (var prop in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    CheckForDocumentMappableAttribute(prop.FieldType, assemblies, seenTypes);
                }
            }
        }

        [DocumentMappable("MyAwesomeName")]
        class ClassWithDifferentMappableName : IDocument
        {
            [Newtonsoft.Json.JsonProperty("ID")]
            public Guid Id { get; }

            public ChildClassWithDifferentMappableName Child { get; }
            public List<ChildClassWithDifferentMappableName> ListOfChild { get; }
            public IReadOnlyList<ChildClassWithDifferentMappableName> ReadOnlyListOfChild { get; }
            public IChildClassWithDifferentMappableName InterfaceOfChild { get; }
            public List<IChildClassWithDifferentMappableName> ListOfInterfaceOfChild { get; }
            public ChildClassWithDifferentMappableNameBase BaseOfChild { get; }
            public IList<ChildClassWithDifferentMappableNameBase> ListOfBaseOfChild { get; }
            public IReadOnlyDictionary<string, ChildClassWithDifferentMappableNameBase> ReadOnlyDictionaryOfBaseChild { get; }
            public ChildClassWithDifferentMappableNameBase[] ArrayOfChild { get; }

            public ClassWithDifferentMappableName(
                Guid id,
                ChildClassWithDifferentMappableName child,
                List<ChildClassWithDifferentMappableName> listOfChild,
                IReadOnlyList<ChildClassWithDifferentMappableName> readOnlyListOfChild,
                IChildClassWithDifferentMappableName interfaceOfChild,
                List<IChildClassWithDifferentMappableName> listOfInterfaceOfChild,
                ChildClassWithDifferentMappableNameBase baseOfChild,
                IList<ChildClassWithDifferentMappableNameBase> listOfBaseOfChild,
                IReadOnlyDictionary<string, ChildClassWithDifferentMappableNameBase> readOnlyDictionaryOfBaseChild,
                ChildClassWithDifferentMappableNameBase[] arrayOfChild)
            {
                Id = id;
                Child = child;
                ListOfChild = listOfChild;
                ReadOnlyListOfChild = readOnlyListOfChild;
                InterfaceOfChild = interfaceOfChild;
                ListOfInterfaceOfChild = listOfInterfaceOfChild;
                BaseOfChild = baseOfChild;
                ListOfBaseOfChild = listOfBaseOfChild;
                ReadOnlyDictionaryOfBaseChild = readOnlyDictionaryOfBaseChild;
                ArrayOfChild = arrayOfChild;
            }
        }
    }

    public static class PropertyInfoAssertionExtensions
    {
        public static AndConstraint<PropertyInfoAssertions> NotBePubliclyWritable(
            this PropertyInfoAssertions assertions,
            string because = "", params object[] becauseArgs)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            var setMethod = assertions.Subject?.GetSetMethod();
#pragma warning restore CA1062 // Validate arguments of public methods

            FluentAssertions.Execution.Execute.Assertion
                .ForCondition(setMethod == null || !setMethod.IsPublic)
                .BecauseOf(because, becauseArgs)
                .FailWith(
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                    "Expected {context:property} {0} not to have a public setter{reason}.",
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                    assertions.Subject);

            return new AndConstraint<PropertyInfoAssertions>(assertions);
        }
    }

    class ClassWithPublicIdSetter : IDocument
    {
        [JsonProperty("Id")]
        public Guid Id { get; set; }
    }

    [DocumentMappable("ClassWithWrongIdJsonPropertyName")]
    class ClassWithWrongIdJsonPropertyName : IDocument
    {
        [Newtonsoft.Json.JsonProperty("ID")]
        public Guid Id => throw new NotImplementedException();
    }

    class ClassWithWrongConstructorParamName
    {
        public ClassWithWrongConstructorParamName(string param1, string param2XXX)
        {
            Param1 = param1;
            Param2 = param2XXX;
        }

        public string Param1 { get; }
        public string Param2 { get; }
    }

    [DocumentMappable("MyAwesomeChildName")]
    class ChildClassWithDifferentMappableName : ChildClassWithDifferentMappableNameBase, IChildClassWithDifferentMappableName
    {
        public ChildClassWithDifferentMappableName(string field) : base(field)
        {
        }
    }

    [DocumentMappable("MyAwesomeChildInterface")]
    interface IChildClassWithDifferentMappableName
    {
        string Field { get; }
    }

    [DocumentMappable("MyAwesomeChildBase")]
    abstract class ChildClassWithDifferentMappableNameBase
    {
        public string Field { get; }

        protected ChildClassWithDifferentMappableNameBase(string field)
        {
            Field = field;
        }
    }
}

