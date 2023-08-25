using Flash.Extensions;
using Flash.Test.EntityChange;
using Flash.Test.StartupTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flash.Test
{
    [TestFixture]
    public class SystemExtensionTests : BaseTest
    {
        [Test]
        public void ToBooleanTest()
        {
            Assert.IsTrue("1".ToBoolean());
            Assert.IsTrue(1.ToBoolean());
            Assert.IsTrue("True".ToBoolean());
            Assert.IsTrue("TRUE".ToBoolean());
            Assert.IsTrue("0".ToBoolean() == false);
            Assert.IsTrue(0.ToBoolean() == false);
            Assert.IsTrue("False".ToBoolean() == false);
            Assert.IsTrue("FALSE".ToBoolean() == false);
            Assert.IsTrue("Example".ToBoolean() == false);
            Assert.IsTrue("".ToBoolean() == false);
            Assert.IsTrue(" ".ToBoolean() == false);
            Assert.IsTrue(string.Empty.ToBoolean() == false);
            Assert.IsTrue(default(String).ToBoolean() == false);
            Assert.IsTrue((1.1).ToBoolean() == false);
        }

        [Test]
        public void ToFloatTest()
        {
            Assert.IsTrue("1".ToFloat() == 1);
            Assert.IsTrue("ABC".ToFloat() == .0f);

            Assert.IsTrue(1.ToFloat() == 1);
            Assert.IsTrue(Int16.MaxValue.ToFloat() == Int16.MaxValue);
            Assert.IsTrue(Int16.MinValue.ToFloat() == Int16.MinValue);
            Assert.IsTrue(Int32.MaxValue.ToFloat() == Int32.MaxValue);
            Assert.IsTrue(Int32.MinValue.ToFloat() == Int32.MinValue);
            Assert.IsTrue(Int64.MaxValue.ToFloat() == Int64.MaxValue);
            Assert.IsTrue(Int64.MinValue.ToFloat() == Int64.MinValue);
            Assert.IsTrue(default(Int16).ToFloat() == .0f);
            Assert.IsTrue(default(Int32).ToFloat() == .0f);
            Assert.IsTrue(default(Int64).ToFloat() == .0f);

            Assert.IsTrue(1f.ToFloat() == 1f);
            Assert.IsTrue(Single.MaxValue.ToFloat() == Single.MaxValue);
            Assert.IsTrue(Single.MinValue.ToFloat() == Single.MinValue);
            Assert.IsTrue(default(Single).ToFloat() == .0f);

            Assert.IsTrue(1d.ToFloat() == 1f);
            Assert.IsTrue(Double.MaxValue.ToFloat() == float.PositiveInfinity);
            Assert.IsTrue(Double.MinValue.ToFloat() == float.NegativeInfinity);
            Assert.IsTrue(default(Double).ToFloat() == .0f);

            Assert.IsTrue(1m.ToFloat() == 1f);
            Assert.IsTrue(Double.MaxValue.ToFloat() == float.PositiveInfinity);
            Assert.IsTrue(Double.MinValue.ToFloat() == float.NegativeInfinity);
            Assert.IsTrue(default(Object).ToFloat() == .0f);

            Assert.IsTrue(default(Object).ToFloat(1f) == 1f);
        }

        [Test]
        public void ToIntTest()
        {
            Assert.IsTrue("1".ToInt() == 1);
            Assert.IsTrue("ABC".ToInt() == 0);

            Assert.IsTrue(1.ToInt() == 1);
            Assert.IsTrue(Int16.MaxValue.ToInt() == Int16.MaxValue);
            Assert.IsTrue(Int16.MinValue.ToInt() == Int16.MinValue);
            Assert.IsTrue(Int32.MaxValue.ToInt() == Int32.MaxValue);
            Assert.IsTrue(Int32.MinValue.ToInt() == Int32.MinValue);
            Assert.IsTrue(Int64.MaxValue.ToInt() == 0);
            Assert.IsTrue(Int64.MinValue.ToInt() == 0);
            Assert.IsTrue(default(Int16).ToInt() == 0);
            Assert.IsTrue(default(Int32).ToInt() == 0);
            Assert.IsTrue(default(Int64).ToInt() == 0);

            Assert.IsTrue(1f.ToInt() == 1);
            Assert.IsTrue(Single.MaxValue.ToInt() == 0);
            Assert.IsTrue(Single.MinValue.ToInt() == 0);
            Assert.IsTrue(default(Single).ToInt() == 0);

            Assert.IsTrue(1d.ToInt() == 1f);
            Assert.IsTrue(Double.MaxValue.ToInt() == 0);
            Assert.IsTrue(Double.MinValue.ToInt() == 0);
            Assert.IsTrue(default(Double).ToInt() == 0);

            Assert.IsTrue(1m.ToInt() == 1f);
            Assert.IsTrue(Double.MaxValue.ToInt() == 0);
            Assert.IsTrue(Double.MinValue.ToInt() == 0);
            Assert.IsTrue(default(Object).ToInt() == 0);

            Assert.IsTrue(default(Object).ToInt(1) == 1);
        }

        [Test]
        public void ToDecimalTest()
        {
            Assert.IsTrue("1".ToDecimal() == 1m);
            Assert.IsTrue("ABC".ToDecimal() == 0m);

            Assert.IsTrue(1.ToDecimal() == 1m);
            Assert.IsTrue(Int16.MaxValue.ToDecimal() == Int16.MaxValue);
            Assert.IsTrue(Int16.MinValue.ToDecimal() == Int16.MinValue);
            Assert.IsTrue(Int32.MaxValue.ToDecimal() == Int32.MaxValue);
            Assert.IsTrue(Int32.MinValue.ToDecimal() == Int32.MinValue);
            Assert.IsTrue(Int64.MaxValue.ToDecimal() == Int64.MaxValue);
            Assert.IsTrue(Int64.MinValue.ToDecimal() == Int64.MinValue);
            Assert.IsTrue(default(Int16).ToDecimal() == 0);
            Assert.IsTrue(default(Int32).ToDecimal() == 0);
            Assert.IsTrue(default(Int64).ToDecimal() == 0);

            Assert.IsTrue(1f.ToDecimal() == 1m);
            Assert.IsTrue(Single.MaxValue.ToDecimal() == 0);
            Assert.IsTrue(Single.MinValue.ToDecimal() == 0);
            Assert.IsTrue(default(Single).ToDecimal() == 0);

            Assert.IsTrue(1d.ToDecimal() == 1m);
            Assert.IsTrue(Double.MaxValue.ToDecimal() == 0);
            Assert.IsTrue(Double.MinValue.ToDecimal() == 0);
            Assert.IsTrue(default(Double).ToDecimal() == 0);

            Assert.IsTrue(1m.ToDecimal() == 1m);
            Assert.IsTrue(Double.MaxValue.ToDecimal() == 0);
            Assert.IsTrue(Double.MinValue.ToDecimal() == 0);
            Assert.IsTrue(default(Object).ToDecimal() == 0);

            Assert.IsTrue(default(Object).ToDecimal(1m) == 1m);
        }

        [Test]
        public void ToListTest()
        {
            Assert.AreEqual(3, "[1,2,3]".ToList<int>().Count);
            Assert.AreEqual(3, "[\"1\",\"2\",\"3\"]".ToList<string>().Count);
            Assert.AreEqual(3, "[\"1\",\"2\",\"3\"]".ToList<EnumTest>().Count);
            Assert.AreEqual(3, "[1,2,3]".ToList<EnumTest>().Count);
        }

        [Test]
        public void ToFirstUpperStrTest()
        {
            Assert.DoesNotThrow(new TestDelegate(() =>
            {
                "".ToFirstUpperStr();
                string.Empty.ToFirstUpperStr();
            }));

            Assert.AreEqual("A", "a".ToFirstUpperStr());
            Assert.AreEqual("Aaaa", "aaaa".ToFirstUpperStr());
        }
        [Test]
        public void ToFirstLowerStrTest()
        {
            Assert.DoesNotThrow(new TestDelegate(() =>
            {
                "".ToFirstLowerStr();
                string.Empty.ToFirstLowerStr();
            }));

            Assert.AreEqual("a", "A".ToFirstLowerStr());
            Assert.AreEqual("aAAA", "AAAA".ToFirstLowerStr());
        }
        [Test]
        public void ToMaskCardIdTest()
        {
            Assert.DoesNotThrow(new TestDelegate(() =>
            {
                "".ToMaskCardId();
                string.Empty.ToMaskCardId();
                "CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC".ToMaskCardId();
            }));

            Assert.AreEqual("******************", "110101200007283300".ToMaskCardId(-1));
            Assert.AreEqual("******************", "110101200007283300".ToMaskCardId(-1, 18));
            Assert.AreEqual("******************", "110101200007283300".ToMaskCardId(0, 18));
            Assert.AreEqual("******************", "110101200007283300".ToMaskCardId(0, 19));
            Assert.AreEqual("******************", "110101200007283300".ToMaskCardId(2, 19));
            Assert.AreEqual("11******0007283300", "110101200007283300".ToMaskCardId(2, 6));
        }
        [Test]
        public void ValidCardIdTest()
        {
            Assert.DoesNotThrow(new TestDelegate(() =>
            {
                "".ValidCardId();
                string.Empty.ValidCardId();
                "CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC".ValidCardId();
            }));

            Assert.IsTrue("110101200007283300".ValidCardId());
            Assert.IsTrue("110101200007283301".ValidCardId() == false);
        }
        [Test]
        public void ConverToEnumTest()
        {
            Assert.AreEqual(EnumTest.One, "1".ConverToEnum<EnumTest>());
            Assert.AreEqual(null, "4".ConverToEnum<EnumTest>());
            Assert.AreEqual(EnumTest.None, "4".ConverToEnum<EnumTest>(EnumTest.None));
        }
        [Test]
        public void DeepCloneTest()
        {
            var id = Guid.NewGuid();
            var value1 = new Student()
            {
                ChangeObjectId = id,
                Id = 1,
                Age = 16,
                Name = null,
                Sex = EStudentSex.Male,
                CreateTime = DateTime.Now,
                Monery = 1,
                Lists = new List<ListObject> { new ListObject { Id = 1, Name = "1" } },
            };

            Assert.AreEqual(value1.DeepClone<Student>().ChangeObjectId, value1.ChangeObjectId);
            Assert.AreEqual(((Student)((Object)value1).DeepClone()).ChangeObjectId, value1.ChangeObjectId);

            var value2 = new
            {
                ChangeObjectId = id,
                Id = 1,
                Age = 16,
                Name = string.Empty,
                Sex = EStudentSex.Male,
                CreateTime = DateTime.Now,
                Monery = 1,
                Lists = new List<ListObject> { new ListObject { Id = 1, Name = "1" } },
            };
            Assert.AreEqual(value2.DeepClone().ChangeObjectId, value2.ChangeObjectId);

            var value3 = new List<Student>() { value1 };
            Assert.AreEqual(value3.DeepClone().FirstOrDefault().ChangeObjectId, value3.FirstOrDefault().ChangeObjectId);

            var value4 = new Dictionary<Guid, Student>() { { id, value1 } };
            var value4Clone = value4.DeepClone<Dictionary<Guid, Student>>();
            Assert.AreEqual(value4.Keys.First(), value4Clone.Keys.First());
            Assert.AreEqual(value4.Values.First().ChangeObjectId, value4Clone.Values.First().ChangeObjectId);
        }

        [Test]
        public void ToUpperTest()
        {
            Assert.AreEqual("壹拾贰万叁仟肆佰伍拾陆元整", 123456m.ToUpper());
            Assert.AreEqual("壹拾贰万叁仟肆佰伍拾陆元壹角贰分", 123456.12m.ToUpper());
            Assert.AreEqual("壹拾贰万叁仟肆佰伍拾陆元壹角贰分", 123456.123m.ToUpper());
        }
    }

    public enum EnumTest
    {
        None = 0,
        One = 1,
        Two = 2,
        Three = 3,
    }
}
