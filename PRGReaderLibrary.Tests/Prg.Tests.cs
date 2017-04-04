﻿namespace PRGReaderLibrary.Tests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.IO;

    [TestFixture]
    public class PrgReader_Tests
    {
        public void VariableVariantToFromTest(VariableVariant value, List<DigitalCustomUnitsPoint> customUnits)
        {
            var tempValue = new VariableVariant(value.ToString(), value.Units, customUnits);
            ObjectAssert.AreEqual(value, tempValue,
                $@"Variable value toFrom string test failed.
Value.ToString(): {value.ToString()}
Value.ToFromToString(): {tempValue.ToString()}
");
        }

        public void BaseTest(string name)
        {
            var path = TestUtilities.GetFullPathForTestFile(name);
            var prg = Prg.Load(path);

            var temp = Path.GetTempFileName();

            //Test variables binary load/save compatible

            //Bit to bit compatible supported only for current version
            if (prg.FileVersion == FileVersion.Current)
            {
                foreach (var input in prg.Inputs)
                {
                    VariableVariantToFromTest(input.Value, prg.CustomUnits);

                    ObjectAssert.AreEqual(input, new InputPoint(input.ToBytes()),
                            $"{nameof(input)} ToFromBytes test failed.");
                }

                foreach (var output in prg.Outputs)
                {
                    VariableVariantToFromTest(output.Value, prg.CustomUnits);

                    ObjectAssert.AreEqual(output, new OutputPoint(output.ToBytes()),
                        $"{nameof(output)} ToFromBytes test failed.");
                }

                foreach (var variable in prg.Variables)
                {
                    VariableVariantToFromTest(variable.Value, prg.CustomUnits);

                    ObjectAssert.AreEqual(variable, new VariablePoint(variable.ToBytes()),
                        $"{nameof(variable)} ToFromBytes test failed.");
                }

                foreach (var program in prg.Programs)
                {
                    ObjectAssert.AreEqual(program, new ProgramPoint(program.ToBytes()),
                        $"{nameof(program)} ToFromBytes test failed.");
                }

                foreach (var controller in prg.Controllers)
                {
                    ObjectAssert.AreEqual(controller, new ControllerPoint(controller.ToBytes()),
                        $"{nameof(controller)} ToFromBytes test failed.");
                }

                foreach (var screen in prg.Screens)
                {
                    ObjectAssert.AreEqual(screen, new ScreenPoint(screen.ToBytes()),
                        $"{nameof(screen)} ToFromBytes test failed.");
                }

                foreach (var graphic in prg.Graphics)
                {
                    ObjectAssert.AreEqual(graphic, new GraphicPoint(graphic.ToBytes()),
                        $"{nameof(graphic)} ToFromBytes test failed.");
                }

                foreach (var user in prg.Users)
                {
                    ObjectAssert.AreEqual(user, new UserPoint(user.ToBytes()),
                        $"{nameof(user)} ToFromBytes test failed.");
                }

                foreach (var unit in prg.CustomUnits)
                {
                    ObjectAssert.AreEqual(unit, new DigitalCustomUnitsPoint(unit.ToBytes()),
                        $"{nameof(unit)} ToFromBytes test failed.");
                }

                foreach (var table in prg.Tables)
                {
                    ObjectAssert.AreEqual(table, new TablePoint(table.ToBytes()),
                        $"{nameof(table)} ToFromBytes test failed.");
                }

                {
                    var settings = prg.Settings;
                    ObjectAssert.AreEqual(settings, new Settings(settings.ToBytes()),
                        $"{nameof(settings)} ToFromBytes test failed.");
                }

                foreach (var schedule in prg.Schedules)
                {
                    ObjectAssert.AreEqual(schedule, new SchedulePoint(schedule.ToBytes()),
                        $"{nameof(schedule)} ToFromBytes test failed.");
                }

                foreach (var holiday in prg.Holidays)
                {
                    ObjectAssert.AreEqual(holiday, new HolidayPoint(holiday.ToBytes()),
                        $"{nameof(holiday)} ToFromBytes test failed.");
                }

                foreach (var monitor in prg.Monitors)
                {
                    ObjectAssert.AreEqual(monitor, new MonitorPoint(monitor.ToBytes()),
                        $"{nameof(monitor)} ToFromBytes test failed.");
                }

                foreach (var code in prg.ProgramCodes)
                {
                    ObjectAssert.AreEqual(code, new ProgramCode(code.ToBytes(), 0),
                        $"{nameof(code)} ToFromBytes test failed.");
                }

                foreach (var units in prg.AnalogCustomUnits)
                {
                    ObjectAssert.AreEqual(units, new AnalogCustomUnitsPoint(units.ToBytes()),
                        $"{nameof(units)} ToFromBytes test failed.");
                }
            }

            prg.Save(temp);
            FileAssert.AreEqual(path, temp, name);

            if (prg.Variables.Count > 0)
            {
                prg = Prg.Load(temp);
                prg.Variables[0].Value = new VariableVariant("9998.8999", Units.DegreesC);
                prg.Save(temp);
                FileAssert.AreNotEqual(path, temp);
            }

            //Not supported while using RawData. 
            //Only updating the file without changing the format is available.
            /*
            //Additional check for upgrade to current
            if (prg.FileVersion != FileVersion.Current)
            {
                prg.Upgrade();
                prg.Save(temp);
                prg = Prg.Load(temp);
                Assert.AreEqual(FileVersion.Current, prg.FileVersion);
            }
            */
        }

        public void UnsupportedTest(string name)
        {
            var exception = Assert.Catch(() =>
            {
                var prg = Prg.Load(TestUtilities.GetFullPathForTestFile(name));

                Console.WriteLine(prg.PropertiesText());
            });
            Console.WriteLine(exception.Message);
        }

        [Test]
        public void Prg_BTUMeter()
        {
            var path = TestUtilities.GetFullPathForTestFile("BTUMeter.prg");
            var prg = Prg.Load(path);

            ObjectAssert.AreEqual(new DigitalCustomUnitsPoint(false, "TANK1", "TANK2"), prg.CustomUnits[0]);
        }

        [Test]
        public void Prg_BaseTests()
        {
            //Current
            BaseTest("BTUMeter.prg");

            //Dos
            BaseTest("asy1.prg");
            BaseTest("panel2.prg");
            BaseTest("panel11.prg");
            BaseTest("panel1.prg");
            BaseTest("testvariables.prg");
            BaseTest("temco.prg");

            //Unsupported
            UnsupportedTest("balsam2.prg");
            UnsupportedTest("90185.prg");
            UnsupportedTest("SelfTestRev3.prg");
            UnsupportedTest("ChamberRev5.prg");
        }

        [Test]
        public void Prg_TestVariables()
        {
            var prg = Prg.Load(TestUtilities.GetFullPathForTestFile("testvariables.prg"));

            var variable1 = prg.Variables[0];
            Assert.AreEqual("FirstDescription    ", variable1.Description);
            Assert.AreEqual("FirstLabe", variable1.Label);
            Assert.AreEqual(new VariableVariant(5.0, Units.DegreesC), variable1.Value);
            Assert.AreEqual(AutoManual.Automatic, variable1.AutoManual);
            Assert.AreEqual(DigitalAnalog.Analog, variable1.DigitalAnalog);
            Assert.AreEqual(Control.Off, variable1.Control);

            var variable2 = prg.Variables[1];
            Assert.AreEqual("SecondDescription   ", variable2.Description);
            Assert.AreEqual("SecondLab", variable2.Label);
            ObjectAssert.AreEqual(new VariableVariant("On", Units.OffOn).Value, variable2.Value.Value);
            Assert.AreEqual(AutoManual.Manual, variable2.AutoManual);
            Assert.AreEqual(DigitalAnalog.Digital, variable2.DigitalAnalog);
            Assert.AreEqual(Control.Off, variable2.Control);

            var variable3 = prg.Variables[2];
            Assert.AreEqual("ThirdDescription    ", variable3.Description);
            Assert.AreEqual("ThirdLabe", variable3.Label);
            Assert.AreEqual(new VariableVariant(new TimeSpan(0, 22, 22, 22, 0), Units.Time), variable3.Value);
            Assert.AreEqual(AutoManual.Automatic, variable3.AutoManual);
            Assert.AreEqual(DigitalAnalog.Analog, variable3.DigitalAnalog);
            Assert.AreEqual(Control.Off, variable3.Control);
        }
    }
}
