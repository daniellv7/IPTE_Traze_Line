/* 
 * Represents different physical units such as frequency, voltage or power.
 * Date:   15/01/2019
 */

using System;
using System.Collections.Generic;

namespace IPTE_Base_Project.Common
{
    public enum FrequencyUnits
    {
        MHz = 0,
        Hz,
        kHz,
        GHz,
        Unknown,
    }

    public struct Frequency
    {
        public double magnitude;
        public FrequencyUnits units;

        public Frequency(double magnitude, FrequencyUnits units)
        {
            this.magnitude = magnitude;
            this.units = units;
        }

        public override string ToString()
        {
            return this.magnitude + this.units.ToString();
        }

        public static bool EqualsWithinAbsolutePrecision(Frequency frequency1, Frequency frequency2, double absPrecission)
        {
            switch (frequency1.units)
            {
                case FrequencyUnits.Hz:
                    break;
                case FrequencyUnits.kHz:
                    frequency1.magnitude = frequency1.magnitude * 1.0E+03;
                    break;
                case FrequencyUnits.MHz:
                    frequency1.magnitude = frequency1.magnitude * 1.0E+06;
                    break;
                case FrequencyUnits.GHz:
                    frequency1.magnitude = frequency1.magnitude * 1.0E+09;
                    break;
                case FrequencyUnits.Unknown:
                    throw new Exception("Unknow unit to compare frequencies.");
                default:
                    throw new Exception("No unit to compare frequencies.");
            }

            switch (frequency2.units)
            {
                case FrequencyUnits.Hz:
                    break;
                case FrequencyUnits.kHz:
                    frequency2.magnitude = frequency2.magnitude * 1.0E+03;
                    break;
                case FrequencyUnits.MHz:
                    frequency2.magnitude = frequency2.magnitude * 1.0E+06;
                    break;
                case FrequencyUnits.GHz:
                    frequency2.magnitude = frequency2.magnitude * 1.0E+09;
                    break;
                case FrequencyUnits.Unknown:
                    throw new Exception("Unknow unit to compare frequencies.");
                default:
                    throw new Exception("No unit to compare frequencies.");
            }

            if (Math.Abs(frequency1.magnitude - frequency2.magnitude) <= absPrecission)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool EqualsWithinRelativePrecision(Frequency frequency1, Frequency frequency2, double relPrecission)
        {
            switch (frequency1.units)
            {
                case FrequencyUnits.Hz:
                    break;
                case FrequencyUnits.kHz:
                    frequency1.magnitude = frequency1.magnitude * 1.0E+03;
                    break;
                case FrequencyUnits.MHz:
                    frequency1.magnitude = frequency1.magnitude * 1.0E+06;
                    break;
                case FrequencyUnits.GHz:
                    frequency1.magnitude = frequency1.magnitude * 1.0E+09;
                    break;
                case FrequencyUnits.Unknown:
                    throw new Exception("Unknow unit to compare frequencies.");
                default:
                    throw new Exception("No unit to compare frequencies.");
            }

            switch (frequency2.units)
            {
                case FrequencyUnits.Hz:
                    break;
                case FrequencyUnits.kHz:
                    frequency2.magnitude = frequency2.magnitude * 1.0E+03;
                    break;
                case FrequencyUnits.MHz:
                    frequency2.magnitude = frequency2.magnitude * 1.0E+06;
                    break;
                case FrequencyUnits.GHz:
                    frequency2.magnitude = frequency2.magnitude * 1.0E+09;
                    break;
                case FrequencyUnits.Unknown:
                    throw new Exception("Unknow unit to compare frequencies.");
                default:
                    throw new Exception("No unit to compare frequencies.");
            }

            double differencePrev = Math.Abs(frequency1.magnitude * relPrecission);
            double difference = Math.Max(differencePrev, relPrecission);
            if (Math.Abs(frequency1.magnitude - frequency2.magnitude) <= difference)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }

    public enum VoltageUnits
    {
        V = 0,
        mV,
        Unknown
    }

    public struct Voltage
    {
        public double magnitude;
        public VoltageUnits units;

        public Voltage(double magnitude, VoltageUnits units)
        {
            this.magnitude = magnitude;
            this.units = units;
        }

        public override string ToString()
        {
            return this.magnitude + this.units.ToString();
        }

        public static bool EqualsWithinRelativePrecision(Voltage voltage1, Voltage voltage2, double relPrecission)
        {
            switch (voltage1.units)
            {
                case VoltageUnits.mV:
                    voltage1.magnitude = voltage1.magnitude * 1.0E-03;
                    break;
                case VoltageUnits.V:
                    break;
                case VoltageUnits.Unknown:
                    throw new Exception("Unknow unit to compare voltages.");
                default:
                    throw new Exception("No unit to compare voltages.");
            }

            switch (voltage2.units)
            {
                case VoltageUnits.mV:
                    voltage2.magnitude = voltage2.magnitude * 1.0E-03;
                    break;
                case VoltageUnits.V:
                    break;
                case VoltageUnits.Unknown:
                    throw new Exception("Unknow unit to compare voltages.");
                default:
                    throw new Exception("No unit to compare voltages.");
            }

            double differencePrev = Math.Abs(voltage1.magnitude * relPrecission);
            double difference = Math.Max(differencePrev, relPrecission);
            if (Math.Abs(voltage1.magnitude - voltage2.magnitude) <= difference)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool VoltageWithinInterval(Voltage actualVoltage, Voltage voltageMin, Voltage voltageMax, double relPrecission)
        {
            switch (actualVoltage.units)
            {
                case VoltageUnits.mV:
                    actualVoltage.magnitude = actualVoltage.magnitude * 1.0E-03;
                    break;
                case VoltageUnits.V:
                    break;
                case VoltageUnits.Unknown:
                    throw new Exception("Unknow unit to compare voltages.");
                default:
                    throw new Exception("No unit to compare voltages.");
            }

            double minrel = 0.0;
            double minabs = 0.0;
            double minPrecission = 0.0;
            double voltageMinCorrectedPrec = 0.0;
            switch (voltageMin.units)
            {
                case VoltageUnits.mV:
                    voltageMin.magnitude = actualVoltage.magnitude * 1.0E-03;
                    minrel = Math.Abs(voltageMin.magnitude * relPrecission);
                    minabs = 1.0E-03 * relPrecission;
                    minPrecission = Math.Max(minrel, minabs);
                    voltageMinCorrectedPrec = voltageMin.magnitude - minPrecission;
                    break;
                case VoltageUnits.V:
                    minrel = Math.Abs(voltageMin.magnitude * relPrecission);
                    minabs = 1.0 * relPrecission;
                    minPrecission = Math.Max(minrel, minabs);
                    voltageMinCorrectedPrec = voltageMin.magnitude - minPrecission;
                    break;
                case VoltageUnits.Unknown:
                    voltageMinCorrectedPrec = -99;
                    break;
                default:
                    throw new Exception("No unit to compare voltages.");
            }

            double maxrel = 0.0;
            double maxabs = 0.0;
            double maxPrecission = 0.0;
            double voltageMaxCorrectedPrec = 0.0;
            switch (voltageMax.units)
            {
                case VoltageUnits.mV:
                    voltageMax.magnitude = actualVoltage.magnitude * 1.0E-03;
                    maxrel = Math.Abs(voltageMax.magnitude * relPrecission);
                    maxabs = 1.0E-03 * relPrecission;
                    maxPrecission = Math.Max(maxrel, maxabs);
                    voltageMaxCorrectedPrec = voltageMax.magnitude + maxPrecission;
                    break;
                case VoltageUnits.V:
                    maxrel = Math.Abs(voltageMax.magnitude * relPrecission);
                    maxabs = 1.0 * relPrecission;
                    maxPrecission = Math.Max(maxrel, maxabs);
                    voltageMaxCorrectedPrec = voltageMax.magnitude + maxPrecission;
                    break;
                case VoltageUnits.Unknown:
                    voltageMaxCorrectedPrec = 99;
                    break;
                default:
                    throw new Exception("No unit to compare voltages.");
            }

            if (actualVoltage.magnitude < voltageMinCorrectedPrec && voltageMinCorrectedPrec != -99)
            {
                return false;
            }

            if (actualVoltage.magnitude > voltageMaxCorrectedPrec && voltageMaxCorrectedPrec != 99)
            {
                return false;
            }

            return true;
        }

        public static Voltage operator +(Voltage voltage1, Voltage voltage2)
        {
            switch (voltage1.units)
            {
                case VoltageUnits.mV:
                    voltage1.magnitude = voltage1.magnitude * 1.0E-03;
                    break;
                case VoltageUnits.V:
                    break;
                case VoltageUnits.Unknown:
                    throw new Exception("Unknow unit to sum voltages.");
                default:
                    throw new Exception("No unit to sum voltages.");
            }

            switch (voltage2.units)
            {
                case VoltageUnits.mV:
                    voltage2.magnitude = voltage2.magnitude * 1.0E-03;
                    break;
                case VoltageUnits.V:
                    break;
                case VoltageUnits.Unknown:
                    throw new Exception("Unknow unit to sum voltages.");
                default:
                    throw new Exception("No unit to sum voltages.");
            }

            return new Voltage(voltage1.magnitude + voltage2.magnitude, VoltageUnits.V);
        }
        public static Voltage ComputeVoltageLimits(Voltage voltage, double relPrecission)
        {
            double rel = 0.0;
            double abs = 0.0;
            double mag = 0.0;
            switch (voltage.units)
            {
                case VoltageUnits.mV:
                    voltage.magnitude = voltage.magnitude * 1.0E-03;
                    rel = Math.Abs(voltage.magnitude * relPrecission);
                    abs = 1.0E-03 * relPrecission;
                    mag = Math.Max(rel, abs);
                    break;
                case VoltageUnits.V:
                    rel = Math.Abs(voltage.magnitude * relPrecission);
                    abs = 1.0 * relPrecission;
                    mag = Math.Max(rel, abs);
                    break;
                case VoltageUnits.Unknown:
                    mag = -99;
                    break;
                default:
                    throw new Exception("No unit to compute voltage limit.");
            }

            return new Voltage(mag, VoltageUnits.V);
        }


        public static Voltage ComputeAverage(List<Voltage> voltages)
        {
            double magnitude = 0.0;
            double acum = 0.0;
            double averageMagnitude = 0.0;
            VoltageUnits averageUnits = VoltageUnits.V;
            foreach (Voltage voltage in voltages)
            {
                switch (voltage.units)
                {
                    case VoltageUnits.mV:
                        magnitude = voltage.magnitude * 1.0E-03;
                        break;
                    case VoltageUnits.V:
                        magnitude = voltage.magnitude;
                        break;
                    case VoltageUnits.Unknown:
                        throw new Exception("Unknow unit to average voltages.");
                    default:
                        throw new Exception("No unit to average voltages.");
                }
                acum += magnitude;
            }
            averageMagnitude = acum / voltages.Count;

            return new Voltage(averageMagnitude, averageUnits);
        }
    }

    public enum CurrentUnits
    {
        mA = 0,
        uA,
        A,
        Unknown,
    }

    public struct Current
    {
        public double magnitude;
        public CurrentUnits units;

        public Current(double magnitude, CurrentUnits units)
        {
            this.magnitude = magnitude;
            this.units = units;
        }

        public override string ToString()
        {
            return magnitude + units.ToString();
        }

        public static bool EqualsWithinAbsolutePrecision(Current current1, Current current2, double absPrecission)
        {
            switch (current1.units)
            {
                case CurrentUnits.uA:
                    current1.magnitude = current1.magnitude * 1.0E-06;
                    break;
                case CurrentUnits.mA:
                    current1.magnitude = current1.magnitude * 1.0E-03;
                    break;
                case CurrentUnits.A:
                    break;
                case CurrentUnits.Unknown:
                    throw new Exception("Unknow unit to compare currents.");
                default:
                    throw new Exception("No unit to compare currents.");
            }

            switch (current2.units)
            {
                case CurrentUnits.uA:
                    current2.magnitude = current2.magnitude * 1.0E-06;
                    break;
                case CurrentUnits.mA:
                    current2.magnitude = current2.magnitude * 1.0E-03;
                    break;
                case CurrentUnits.A:
                    break;
                case CurrentUnits.Unknown:
                    throw new Exception("Unknow unit to compare currents.");
                default:
                    throw new Exception("No unit to compare currents.");
            }

            if (Math.Abs(current1.magnitude - current2.magnitude) <= absPrecission)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool EqualsWithinRelativePrecision(Current current1, Current current2, double relPrecission)
        {
            switch (current1.units)
            {
                case CurrentUnits.uA:
                    current1.magnitude = current1.magnitude * 1.0E-06;
                    break;
                case CurrentUnits.mA:
                    current1.magnitude = current1.magnitude * 1.0E-03;
                    break;
                case CurrentUnits.A:
                    break;
                case CurrentUnits.Unknown:
                    throw new Exception("Unknow unit to compare currents.");
                default:
                    throw new Exception("No unit to compare currents.");
            }

            switch (current2.units)
            {
                case CurrentUnits.uA:
                    current2.magnitude = current2.magnitude * 1.0E-06;
                    break;
                case CurrentUnits.mA:
                    current2.magnitude = current2.magnitude * 1.0E-03;
                    break;
                case CurrentUnits.A:
                    break;
                case CurrentUnits.Unknown:
                    throw new Exception("Unknow unit to compare currents.");
                default:
                    throw new Exception("No unit to compare currents.");
            }

            double differencePrev = Math.Abs(current1.magnitude * relPrecission);
            double difference = Math.Max(differencePrev, relPrecission);
            if (Math.Abs(current1.magnitude - current2.magnitude) <= difference)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CurrentWithinInterval(Current actualCurrent, Current currentMin, Current currentMax, double relPrecission)
        {
            switch (actualCurrent.units)
            {
                case CurrentUnits.uA:
                    actualCurrent.magnitude = actualCurrent.magnitude * 1.0E-06;
                    break;
                case CurrentUnits.mA:
                    actualCurrent.magnitude = actualCurrent.magnitude * 1.0E-03;
                    break;
                case CurrentUnits.A:
                    break;
                case CurrentUnits.Unknown:
                    throw new Exception("Unknow unit to compare currents.");
                default:
                    throw new Exception("No unit to compare currents.");
            }

            double minrel = 0.0;
            double minabs = 0.0;
            double minPrecission = 0.0;
            double currentMinCorrectedPrec = 0.0;
            switch (currentMin.units)
            {
                case CurrentUnits.uA:
                    currentMin.magnitude = currentMin.magnitude * 1.0E-06;
                    minrel = Math.Abs(currentMin.magnitude * relPrecission);
                    minabs = 1.0E-06 * relPrecission;
                    minPrecission = Math.Max(minrel, minabs);
                    currentMinCorrectedPrec = currentMin.magnitude - minPrecission;
                    break;
                case CurrentUnits.mA:
                    currentMin.magnitude = currentMin.magnitude * 1.0E-03;
                    minrel = Math.Abs(currentMin.magnitude * relPrecission);
                    minabs = 1.0E-03 * relPrecission;
                    minPrecission = Math.Max(minrel, minabs);
                    currentMinCorrectedPrec = currentMin.magnitude - minPrecission;
                    break;
                case CurrentUnits.A:
                    minrel = Math.Abs(currentMin.magnitude * relPrecission);
                    minabs = 1.0 * relPrecission;
                    minPrecission = Math.Max(minrel, minabs);
                    currentMinCorrectedPrec = currentMin.magnitude - minPrecission;
                    break;
                case CurrentUnits.Unknown:
                    throw new Exception("Unknow unit to compare currents.");
                default:
                    throw new Exception("No unit to compare currents.");
            }

            double maxrel = 0.0;
            double maxabs = 0.0;
            double maxPrecission = 0.0;
            double currentMaxCorrectedPrec = 0.0;
            switch (currentMax.units)
            {
                case CurrentUnits.uA:
                    currentMax.magnitude = currentMax.magnitude * 1.0E-06;
                    maxrel = Math.Abs(currentMax.magnitude * relPrecission);
                    maxabs = 1.0E-06 * relPrecission;
                    maxPrecission = Math.Max(minrel, minabs);
                    currentMaxCorrectedPrec = currentMax.magnitude + maxPrecission;
                    break;
                case CurrentUnits.mA:
                    currentMax.magnitude = currentMax.magnitude * 1.0E-03;
                    maxrel = Math.Abs(currentMax.magnitude * relPrecission);
                    maxabs = 1.0E-03 * relPrecission;
                    maxPrecission = Math.Max(minrel, minabs);
                    currentMaxCorrectedPrec = currentMax.magnitude + maxPrecission;
                    break;
                case CurrentUnits.A:
                    maxrel = Math.Abs(currentMax.magnitude * relPrecission);
                    maxabs = 1.0 * relPrecission;
                    maxPrecission = Math.Max(minrel, minabs);
                    currentMaxCorrectedPrec = currentMax.magnitude + maxPrecission;
                    break;
                case CurrentUnits.Unknown:
                    throw new Exception("Unknow unit to compare currents.");
                default:
                    throw new Exception("No unit to compare currents.");
            }

            if (actualCurrent.magnitude < currentMinCorrectedPrec)
            {
                return false;
            }

            if (actualCurrent.magnitude > currentMaxCorrectedPrec)
            {
                return false;
            }

            return true;
        }

        public static Current ComputeCurrentLimits(Current current, double relPrecission)
        {
            double rel = 0.0;
            double abs = 0.0;
            double mag = 0.0;
            switch (current.units)
            {
                case CurrentUnits.uA:
                    current.magnitude = current.magnitude * 1.0E-06;
                    rel = Math.Abs(current.magnitude * relPrecission);
                    abs = 1.0E-06 * relPrecission;
                    mag = Math.Max(rel, abs);
                    break;
                case CurrentUnits.mA:
                    current.magnitude = current.magnitude * 1.0E-03;
                    rel = Math.Abs(current.magnitude * relPrecission);
                    abs = 1.0E-03 * relPrecission;
                    mag = Math.Max(rel, abs);
                    break;
                case CurrentUnits.A:
                    rel = Math.Abs(current.magnitude * relPrecission);
                    abs = 1.0 * relPrecission;
                    mag = Math.Max(rel, abs);
                    break;
                case CurrentUnits.Unknown:
                    throw new Exception("Unknow unit to compute current limit.");
                default:
                    throw new Exception("No unit to compute current limit.");
            }

            return new Current(mag, CurrentUnits.A);
        }

        public static Current ComputeAverage(List<Current> currents)
        {
            double magnitude = 0.0;
            double acum = 0.0;
            double averageMagnitude = 0.0;
            CurrentUnits averageUnits = CurrentUnits.A;
            foreach (Current current in currents)
            {
                switch (current.units)
                {
                    case CurrentUnits.uA:
                        magnitude = current.magnitude * 1.0E-06;
                        break;
                    case CurrentUnits.mA:
                        magnitude = current.magnitude * 1.0E-03;
                        break;
                    case CurrentUnits.A:
                        magnitude = current.magnitude;
                        break;
                    case CurrentUnits.Unknown:
                        throw new Exception("Unknow unit to average currents.");
                    default:
                        throw new Exception("No unit to average currents.");
                }
                acum += magnitude;
            }
            averageMagnitude = acum / currents.Count;

            return new Current(averageMagnitude, averageUnits);
        }
    }

    public enum TimeUnits
    {
        ms = 0,
        s,
        Unknown
    }

    public struct Time
    {
        public int magnitude;
        public TimeUnits units;

        public Time(int magnitude, TimeUnits units)
        {
            this.magnitude = magnitude;
            this.units = units;
        }

        public override string ToString()
        {
            return this.magnitude + this.units.ToString();
        }

        public int ToMiliseconds()
        {
            switch (units)
            {
                case TimeUnits.ms:
                    return magnitude;
                case TimeUnits.s:
                    return magnitude * 1000;
                case TimeUnits.Unknown:
                    throw new Exception("Unknow time unit to convert to miliseconds.");
                default:
                    throw new Exception("No time unit to convert to miliseconds.");
            }
        }
    }

    public enum PowerUnits
    {
        dBm = 0, //decibel-miliwatt
        Unknown
    }

    public struct Power
    {
        public double magnitude;
        public PowerUnits units;

        public Power(double magnitude, PowerUnits units)
        {
            this.magnitude = magnitude;
            this.units = units;
        }

        public override string ToString()
        {
            return this.magnitude + this.units.ToString();
        }

        public static bool EqualsWithinAbsolutePrecision(Power power1, Power power2, double absPrecission)
        {
            switch (power1.units)
            {
                case PowerUnits.dBm:
                    break;
                case PowerUnits.Unknown:
                    throw new Exception("Unknow unit to compare powers.");
                default:
                    throw new Exception("No unit to compare powers.");
            }

            switch (power2.units)
            {
                case PowerUnits.dBm:
                    break;
                case PowerUnits.Unknown:
                    throw new Exception("Unknow unit to compare powers.");
                default:
                    throw new Exception("No unit to compare powers.");
            }

            if (Math.Abs(power1.magnitude - power2.magnitude) <= absPrecission)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool EqualsWithinRelativePrecision(Power power1, Power power2, double relPrecission)
        {
            switch (power1.units)
            {
                case PowerUnits.dBm:
                    break;
                case PowerUnits.Unknown:
                    throw new Exception("Unknow unit to compare powers.");
                default:
                    throw new Exception("No unit to compare powers.");
            }

            switch (power2.units)
            {
                case PowerUnits.dBm:
                    break;
                case PowerUnits.Unknown:
                    throw new Exception("Unknow unit to compare powers.");
                default:
                    throw new Exception("No unit to compare powers.");
            }

            double differencePrev = Math.Abs(power1.magnitude * relPrecission);
            double difference = Math.Max(differencePrev, relPrecission);
            if (Math.Abs(power1.magnitude - power2.magnitude) <= difference)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int Compare(Power power1, Power power2)
        {
            switch (power1.units)
            {
                case PowerUnits.dBm:
                    break;
                case PowerUnits.Unknown:
                    throw new Exception("Unknow unit to compare powers.");
                default:
                    throw new Exception("No unit to compare powers.");
            }

            switch (power2.units)
            {
                case PowerUnits.dBm:
                    break;
                case PowerUnits.Unknown:
                    throw new Exception("Unknow unit to compare powers.");
                default:
                    throw new Exception("No unit to compare powers.");
            }

            if (power1.magnitude < power2.magnitude)
            {
                return -1;
            }
            if (power1.magnitude > power2.magnitude)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    public enum ResistanceUnits
    {
        Ohms = 0,
        mOhms,
        kOhms,
        Unknown
    }

    public struct Resistance
    {
        public double magnitude;
        public ResistanceUnits units;

        public Resistance(double magnitude, ResistanceUnits units)
        {
            this.magnitude = magnitude;
            this.units = units;
        }

        public override string ToString()
        {
            return this.magnitude + this.units.ToString();
        }

        public static Resistance Parse(string s)
        {
            double magnitude;
            ResistanceUnits units;
            
            s = s.Trim();

            //space between magnitude and units
            if (s.Contains(" "))
            {
                string[] res = s.Split(' ');
                magnitude = double.Parse(res[0]);
                units = (ResistanceUnits)Enum.Parse(typeof(ResistanceUnits), res[1]);
                return new Resistance(magnitude, units);
            }
            //no space between magnitude and units
            else
            {
                if (s.EndsWith("kOhms"))
                {
                    int unitsLength = "kOhms".Length;
                    magnitude = double.Parse(s.Substring(0, s.Length - unitsLength));
                    units = ResistanceUnits.kOhms;
                    return new Resistance(magnitude, units);
                }
                else if (s.EndsWith("mOhms"))
                {
                    int unitsLength = "mOhms".Length;
                    magnitude = double.Parse(s.Substring(0, s.Length - unitsLength));
                    units = ResistanceUnits.mOhms;
                    return new Resistance(magnitude, units);
                }
                else if (s.EndsWith("Ohms"))
                {
                    int unitsLength = "Ohms".Length;
                    magnitude = double.Parse(s.Substring(0, s.Length - unitsLength));
                    units = ResistanceUnits.Ohms;
                    return new Resistance(magnitude, units);
                }
            }
            throw new Exception($"Cannot convert {s} into resistance.");
        }

        public static bool EqualsWithinRelativePrecision(Resistance resistance1, Resistance resistance2, double relPrecission)
        {
            switch (resistance1.units)
            {
                case ResistanceUnits.mOhms:
                    resistance1.magnitude = resistance1.magnitude * 1.0E-03;
                    break;
                case ResistanceUnits.Ohms:
                    break;
                case ResistanceUnits.kOhms:
                    resistance1.magnitude = resistance1.magnitude * 1.0E+03;
                    break;
                case ResistanceUnits.Unknown:
                    throw new Exception("Unknow unit to compare resistances.");
                default:
                    throw new Exception("No unit to compare resistances.");
            }

            switch (resistance2.units)
            {
                case ResistanceUnits.mOhms:
                    resistance2.magnitude = resistance2.magnitude * 1.0E-03;
                    break;
                case ResistanceUnits.Ohms:
                    break;
                case ResistanceUnits.kOhms:
                    resistance2.magnitude = resistance2.magnitude * 1.0E+03;
                    break;
                case ResistanceUnits.Unknown:
                    throw new Exception("Unknow unit to compare resistances.");
                default:
                    throw new Exception("No unit to compare resistances.");
            }

            double differencePrev = Math.Abs(resistance1.magnitude * relPrecission);
            double difference = Math.Max(differencePrev, relPrecission);
            if (Math.Abs(resistance1.magnitude - resistance2.magnitude) <= difference)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ResistanceWithinInterval(Resistance actualResistance, Resistance resistanceMin, Resistance resistanceMax, double relPrecission)
        {
            switch (actualResistance.units)
            {
                case ResistanceUnits.mOhms:
                    actualResistance.magnitude = actualResistance.magnitude * 1.0E-03;
                    break;
                case ResistanceUnits.Ohms:
                    break;
                case ResistanceUnits.kOhms:
                    actualResistance.magnitude = actualResistance.magnitude * 1.0E+03;
                    break;
                case ResistanceUnits.Unknown:
                    throw new Exception("Unknow unit to compare resistances.");
                default:
                    throw new Exception("No unit to compare resistances.");
            }

            double minrel = 0.0;
            double minabs = 0.0;
            double minPrecission = 0.0;
            double resistanceMinCorrectedPrec = 0.0;
            switch (resistanceMin.units)
            {
                case ResistanceUnits.mOhms:
                    resistanceMin.magnitude = actualResistance.magnitude * 1.0E-03;
                    minrel = Math.Abs(resistanceMin.magnitude * relPrecission);
                    minabs = 1.0E-03 * relPrecission;
                    minPrecission = Math.Max(minrel, minabs);
                    resistanceMinCorrectedPrec = resistanceMin.magnitude - minPrecission;
                    break;
                case ResistanceUnits.Ohms:
                    minrel = Math.Abs(resistanceMin.magnitude * relPrecission);
                    minabs = 1.0 * relPrecission;
                    minPrecission = Math.Max(minrel, minabs);
                    resistanceMinCorrectedPrec = resistanceMin.magnitude - minPrecission;
                    break;
                case ResistanceUnits.kOhms:
                    resistanceMin.magnitude = actualResistance.magnitude * 1.0E+03;
                    minrel = Math.Abs(resistanceMin.magnitude * relPrecission);
                    minabs = 1.0E+03 * relPrecission;
                    minPrecission = Math.Max(minrel, minabs);
                    resistanceMinCorrectedPrec = resistanceMin.magnitude - minPrecission;
                    break;
                case ResistanceUnits.Unknown:
                    resistanceMinCorrectedPrec = -99;
                    break;
                default:
                    throw new Exception("No unit to compare resistances.");
            }

            double maxrel = 0.0;
            double maxabs = 0.0;
            double maxPrecission = 0.0;
            double resistanceMaxCorrectedPrec = 0.0;
            switch (resistanceMax.units)
            {
                case ResistanceUnits.mOhms:
                    resistanceMax.magnitude = actualResistance.magnitude * 1.0E-03;
                    maxrel = Math.Abs(resistanceMax.magnitude * relPrecission);
                    maxabs = 1.0E-03 * relPrecission;
                    maxPrecission = Math.Max(maxrel, maxabs);
                    resistanceMaxCorrectedPrec = resistanceMax.magnitude + maxPrecission;
                    break;
                case ResistanceUnits.Ohms:
                    maxrel = Math.Abs(resistanceMax.magnitude * relPrecission);
                    maxabs = 1.0 * relPrecission;
                    maxPrecission = Math.Max(maxrel, maxabs);
                    resistanceMaxCorrectedPrec = resistanceMax.magnitude + maxPrecission;
                    break;
                case ResistanceUnits.kOhms:
                    resistanceMax.magnitude = actualResistance.magnitude * 1.0E+03;
                    maxrel = Math.Abs(resistanceMax.magnitude * relPrecission);
                    maxabs = 1.0E+03 * relPrecission;
                    maxPrecission = Math.Max(maxrel, maxabs);
                    resistanceMaxCorrectedPrec = resistanceMax.magnitude + maxPrecission;
                    break;
                case ResistanceUnits.Unknown:
                    resistanceMaxCorrectedPrec = 99;
                    break;
                default:
                    throw new Exception("No unit to compare resistances.");
            }

            if (actualResistance.magnitude < resistanceMinCorrectedPrec && resistanceMinCorrectedPrec != -99)
            {
                return false;
            }

            if (actualResistance.magnitude > resistanceMaxCorrectedPrec && resistanceMaxCorrectedPrec != 99)
            {
                return false;
            }

            return true;
        }

        public static Resistance operator +(Resistance resistance1, Resistance resistance2)
        {
            switch (resistance1.units)
            {
                case ResistanceUnits.mOhms:
                    resistance1.magnitude = resistance1.magnitude * 1.0E-03;
                    break;
                case ResistanceUnits.Ohms:
                    break;
                case ResistanceUnits.kOhms:
                    resistance1.magnitude = resistance1.magnitude * 1.0E+03;
                    break;
                case ResistanceUnits.Unknown:
                    throw new Exception("Unknow unit to sum resistances.");
                default:
                    throw new Exception("No unit to sum resistances.");
            }

            switch (resistance2.units)
            {
                case ResistanceUnits.mOhms:
                    resistance2.magnitude = resistance2.magnitude * 1.0E-03;
                    break;
                case ResistanceUnits.Ohms:
                    break;
                case ResistanceUnits.kOhms:
                    resistance2.magnitude = resistance2.magnitude * 1.0E+03;
                    break;
                case ResistanceUnits.Unknown:
                    throw new Exception("Unknow unit to sum resistances.");
                default:
                    throw new Exception("No unit to sum resistances.");
            }

            return new Resistance(resistance1.magnitude + resistance2.magnitude, ResistanceUnits.Ohms);
        }
        public static bool operator <(Resistance resistance1, Resistance resistance2)
        {
            switch (resistance1.units)
            {
                case ResistanceUnits.mOhms:
                    resistance1.magnitude = resistance1.magnitude * 1.0E-03;
                    break;
                case ResistanceUnits.Ohms:
                    break;
                case ResistanceUnits.kOhms:
                    resistance1.magnitude = resistance1.magnitude * 1.0E+03;
                    break;
                case ResistanceUnits.Unknown:
                    throw new Exception("Unknow unit to sum resistances.");
                default:
                    throw new Exception("No unit to sum resistances.");
            }

            switch (resistance2.units)
            {
                case ResistanceUnits.mOhms:
                    resistance2.magnitude = resistance2.magnitude * 1.0E-03;
                    break;
                case ResistanceUnits.Ohms:
                    break;
                case ResistanceUnits.kOhms:
                    resistance2.magnitude = resistance2.magnitude * 1.0E+03;
                    break;
                case ResistanceUnits.Unknown:
                    throw new Exception("Unknow unit to sum resistances.");
                default:
                    throw new Exception("No unit to sum resistances.");
            }

            return resistance1.magnitude < resistance2.magnitude;
        }
        public static bool operator >(Resistance resistance1, Resistance resistance2)
        {
            switch (resistance1.units)
            {
                case ResistanceUnits.mOhms:
                    resistance1.magnitude = resistance1.magnitude * 1.0E-03;
                    break;
                case ResistanceUnits.Ohms:
                    break;
                case ResistanceUnits.kOhms:
                    resistance1.magnitude = resistance1.magnitude * 1.0E+03;
                    break;
                case ResistanceUnits.Unknown:
                    throw new Exception("Unknow unit to sum resistances.");
                default:
                    throw new Exception("No unit to sum resistances.");
            }

            switch (resistance2.units)
            {
                case ResistanceUnits.mOhms:
                    resistance2.magnitude = resistance2.magnitude * 1.0E-03;
                    break;
                case ResistanceUnits.Ohms:
                    break;
                case ResistanceUnits.kOhms:
                    resistance2.magnitude = resistance2.magnitude * 1.0E+03;
                    break;
                case ResistanceUnits.Unknown:
                    throw new Exception("Unknow unit to sum resistances.");
                default:
                    throw new Exception("No unit to sum resistances.");
            }

            return resistance1.magnitude > resistance2.magnitude;
        }
        public static bool operator >=(Resistance resistance1, Resistance resistance2)
        {
            return !(resistance1 < resistance2);
        }
        public static bool operator <=(Resistance resistance1, Resistance resistance2)
        {
            return !(resistance1 > resistance2);
        }

        public static Resistance ComputeResistanceLimits(Resistance resistance, double relPrecission)
        {
            double rel = 0.0;
            double abs = 0.0;
            double mag = 0.0;
            switch (resistance.units)
            {
                case ResistanceUnits.mOhms:
                    resistance.magnitude = resistance.magnitude * 1.0E-03;
                    rel = Math.Abs(resistance.magnitude * relPrecission);
                    abs = 1.0E-03 * relPrecission;
                    mag = Math.Max(rel, abs);
                    break;
                case ResistanceUnits.Ohms:
                    rel = Math.Abs(resistance.magnitude * relPrecission);
                    abs = 1.0 * relPrecission;
                    mag = Math.Max(rel, abs);
                    break;
                case ResistanceUnits.kOhms:
                    resistance.magnitude = resistance.magnitude * 1.0E+03;
                    rel = Math.Abs(resistance.magnitude * relPrecission);
                    abs = 1.0E+03 * relPrecission;
                    mag = Math.Max(rel, abs);
                    break;
                case ResistanceUnits.Unknown:
                    mag = -99;
                    break;
                default:
                    throw new Exception("No unit to compute resistance limit.");
            }

            return new Resistance(mag, ResistanceUnits.Ohms);
        }

        public static Resistance ComputeAverage(List<Resistance> resistances)
        {
            double magnitude = 0.0;
            double acum = 0.0;
            double averageMagnitude = 0.0;
            ResistanceUnits averageUnits = ResistanceUnits.Ohms;
            foreach (Resistance resistance in resistances)
            {
                switch (resistance.units)
                {
                    case ResistanceUnits.mOhms:
                        magnitude = resistance.magnitude * 1.0E-03;
                        break;
                    case ResistanceUnits.Ohms:
                        magnitude = resistance.magnitude;
                        break;
                    case ResistanceUnits.kOhms:
                        magnitude = resistance.magnitude * 1.0E+03;
                        break;
                    case ResistanceUnits.Unknown:
                        throw new Exception("Unknow unit to average resistances.");
                    default:
                        throw new Exception("No unit to average resistances.");
                }
                acum += magnitude;
            }
            averageMagnitude = acum / resistances.Count;

            return new Resistance(averageMagnitude, averageUnits);
        }
    }
}
