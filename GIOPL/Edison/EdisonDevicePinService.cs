using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIOPL.Edison
{
    public class EdisonDevicePinService : IDeviceGPIOPinService, IDevicePWMPinService
    {
        private const string GPIOFSLocation = "/sys/class/gpio/";
        private const string GPIOFSExportLocation = GPIOFSLocation + "/export";
        private const string GPIOFSUnExportLocation = GPIOFSLocation + "/unexport";
        private const string PinMuxFSLocation = "/sys/kernel/debug/gpio_debug/gpio{0}/current_pinmux";
        private const string PWMFSExportLocation = "/sys/class/pwm/pwmchip{0}/export";
        private const string PWMFSUnExportLocation = "/sys/class/pwm/pwmchip{0}/unexport";
        private const string PWMFSLocation = "/sys/class/pwm/pwmchip{0}/pwm{1}/";


        private void WriteToFSLocation(string path, string value)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.Write(value);
            }
        }

        private string ReadFromFSLocation(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                return sr.ReadToEnd().Trim();
            }
        }


        public bool ExportGPIOPin(int pin)
        {
            WriteToFSLocation(GPIOFSExportLocation, pin.ToString());

            return GPIOPinExported(pin);
        }

        public bool UnExportGPIOPin(int pin)
        {
            WriteToFSLocation(GPIOFSUnExportLocation, pin.ToString());

            return !GPIOPinExported(pin);
        }

        public bool GPIOPinExported(int pin) => Directory.Exists(GPIOFSLocation + "gpio" + pin.ToString());

        public void SetPinMux(int pin, int mode)
        {
            WriteToFSLocation(string.Format(PinMuxFSLocation, pin.ToString()), "mode" + mode.ToString());
        }

        public bool SetDirection(int pin, PinDirection direction)
        {
            WriteToFSLocation(GPIOFSLocation + "gpio" + pin.ToString() + "/direction", Enum.GetName(typeof(PinDirection), direction).Trim().ToLower());

            return GetDirection(pin) == direction;
        }

        public PinDirection GetDirection(int pin) => (PinDirection)Enum.Parse(typeof(PinDirection), ReadFromFSLocation(GPIOFSLocation + "gpio" + pin.ToString() + "/direction"), true);

        public void SetValue(int pin, PinValue value)
        {
            WriteToFSLocation(GPIOFSLocation + "gpio" + pin.ToString() + "/value", ((int)value).ToString());
        }

        public PinValue GetValue(int pin) => (PinValue)int.Parse(ReadFromFSLocation(GPIOFSLocation + "gpio" + pin.ToString() + "/value"));

        public bool ExportPWMChannel(int channel, int pwmChip = 0)
        {
            WriteToFSLocation(string.Format(PWMFSExportLocation, pwmChip), channel.ToString());
            return PWMChannelExported(channel, pwmChip);
        }

        public bool UnExportPWMChannel(int channel, int pwmChip = 0)
        {
            WriteToFSLocation(string.Format(PWMFSUnExportLocation, pwmChip), channel.ToString());
            return PWMChannelExported(channel, pwmChip);
        }

        public bool PWMChannelExported(int channel, int pwmChip = 0) => Directory.Exists(string.Format(PWMFSLocation, pwmChip, channel));

        public void SetPWMPeriod(int channel, int period, int pwmChip = 0)
        {
            WriteToFSLocation(string.Format(PWMFSLocation, pwmChip, channel) + "period", period.ToString());
        }

        public void SetPWMDutyCycle(int channel, int dutyCycle, int pwmChip = 0)
        {
            WriteToFSLocation(string.Format(PWMFSLocation, pwmChip, channel) + "duty_cycle", dutyCycle.ToString());
        }

        public void EnablePWMChannel(int channel, int pwmChip = 0)
        {
            WriteToFSLocation(string.Format(PWMFSLocation, pwmChip, channel) + "enable", "1");
        }

        public void DisablePWMChannel(int channel, int pwmChip = 0)
        {
            WriteToFSLocation(string.Format(PWMFSLocation, pwmChip, channel) + "enable", "0");
        }
    }
}
