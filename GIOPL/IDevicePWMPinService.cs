namespace GIOPL
{
    public interface IDevicePWMPinService
    {
        void EnablePWMChannel(int channel, int pwmChip = 0);
        bool ExportPWMChannel(int channel, int pwmChip = 0);
        bool PWMChannelExported(int channel, int pwmChip = 0);
        void SetPWMDutyCycle(int channel, int dutyCycle, int pwmChip = 0);
        void SetPWMPeriod(int channel, int period, int pwmChip = 0);
        bool UnExportPWMChannel(int channel, int pwmChip = 0);
        void DisablePWMChannel(int channel, int pwmChip = 0);
    }
}