using AutoFixture;
using AutoFixture.AutoNSubstitute;
using NUnit.Framework;
using System.Diagnostics;

namespace Cyanometer.AirQualityTest;

public abstract class BaseTest<T>
    where T : class
{
    protected Fixture fixture = default!;
    T? target;
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
