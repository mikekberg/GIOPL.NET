namespace GIOPL
{
    public interface IDeviceGPIOPinService
    {
        bool ExportGPIOPin(int pin);
        PinDirection GetDirection(int pin);
        PinValue GetValue(int pin);
        bool GPIOPinExported(int pin);
        bool SetDirection(int pin, PinDirection direction);
        void SetPinMux(int pin, int mode);
        void SetValue(int pin, PinValue value);
    }
}