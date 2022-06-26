using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Cyanometer.Core;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;

namespace Cyanometer.WebTest
{
    public abstract class BaseTest<T>
    where T : class
    {
        protected Fixture fixture;
        T target;
        public T Target
        {
            [DebuggerStepThrough]
            get
            {
                if (target is null)
                {
                    target = fixture.Build<T>().OmitAutoProperties().Create();
                }
                return target;
            }
        }

        [SetUp]
        public virtual void SetUp()
        {
            fixture = new Fixture();
            fixture.Customize(new AutoNSubstituteCustomization());
            // some fake GUIDs
            UploadTokens.Init(
                "C14A3DB5-BEE5-4612-9F9C-62972B2C6C83", 
                "72DCCD9B-B71B-4B73-8A06-EC137C44F49E",
                "161A6F24-66F6-41F9-BC7D-8BA8A949159A", 
                "4AF854D9-647E-4DB0-9219-AD78D64C058D");
        }
        [TearDown]
        public void TearDown()
        {
            target = null;
        }

        public MemoryStream WritesTextToMemoryStream(string text)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream, leaveOpen: true))
            {
                writer.Write(text);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
