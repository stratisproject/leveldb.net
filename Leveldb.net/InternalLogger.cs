using System;

namespace LevelDB
{
    public class Stringable
    {
        public static implicit operator Stringable(int v) => new StringableInt(v);
        public static implicit operator Stringable(byte[] v) => new StringableByteArray(v);
    }

    public class Stringable<T> : Stringable
    {
        protected T _value;


        public Stringable(T value)
        {
            _value = value;
        }
    }

    public class StringableInt : Stringable<int>
    {
        public StringableInt(int v) : base(v)
        {
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }

    public class StringableByteArray : Stringable<byte[]>
    {
        public StringableByteArray(byte[] v) : base(v)
        {
        }

        public override string ToString()
        {
            return BitConverter.ToString(_value);
        }
    }

    public class InternalLogger
    {
        static NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();

        public bool IsDebugEnabled
        {
            get; set;
        }


        public static InternalLogger Create()
        {
            return new InternalLogger();
        }

        public void Debug(string message, params Stringable[] args)
        {
            if (this.IsDebugEnabled)
            {
                _Logger.Debug(message, args);
            }
        }

        public void Info(string message, params Stringable[] args)
        {
            _Logger.Info(message, args);
        }

        public void Error(string message, params Stringable[] args)
        {
            _Logger.Error(message, args);
        }
    }
}
