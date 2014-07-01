using System.Collections.Generic;
using System.Data;

using NUnit.Framework;

namespace pitermarx.IoC
{
    [TestFixture]
    public class IoCTests
    {
        [Test]
        public void IoCIntegrationTests()
        {
            Assert.IsEmpty(Factory.Instances, "Instances starts empty");
            
            // if doesn't exist, create one
            var cont = Factory.GetContainer("TestContainer");

            Assert.IsNotEmpty(Factory.Instances, "TestContainer was created");
            
            // register two instances of the IoCTests Class. One with default name, the other with the name "2"
            cont.Register(() => this);
            cont.Register(() => new IoCTests(), "2");

            Assert.AreSame(cont.Get<IoCTests>(), this, "Default instance is 'this'");
            Assert.AreNotSame(cont.Get<IoCTests>("2"), this, "Named instance is not 'this'");
            
            Assert.Throws<KeyNotFoundException>(() => cont.Get<IoCTests>("3"), "Named instance '3' doesn't exist");

            // seal container to prevent further registers
            cont.Seal();
            
            Assert.Throws<ConstraintException>(() => cont.Register(() => this), "Cannot register. Container is sealed");
            
            // assert i can still get registered instances
            Assert.AreSame(cont.Get<IoCTests>(), this);
            Assert.AreNotSame(cont.Get<IoCTests>("2"), this);
            // and the container still exists
            Assert.IsNotEmpty(Factory.Instances);

            // dispose container
            Factory.DisposeContainer(cont);
            
            Assert.IsEmpty(Factory.Instances, "All the containers have been disposed");
            Assert.Throws<KeyNotFoundException>(() => cont.Get<IoCTests>(), "This container has been disposed");
        }
    }
}