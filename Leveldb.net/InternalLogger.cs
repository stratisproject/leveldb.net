﻿using System;
using System.Linq;

namespace LevelDB
{
    public class Stringable
    {
        public static implicit operator Stringable(int v) => new Stringable<int>(v);
        public static implicit operator Stringable(byte[] v) => new StringableByteArray(v);
        public static implicit operator Stringable(int[] v) => new StringableIntArray(v);
        public static implicit operator Stringable(IntPtr v) => new Stringable<IntPtr>(v);
        public static implicit operator Stringable(string v) => new Stringable<string>(v);
    }

    public class Stringable<T> : Stringable
    {
        protected T _value;


        public Stringable(T value)
        {
            _value = value;
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

    public class StringableIntArray : Stringable<int[]>
    {
        public StringableIntArray(int[] v) : base(v)
        {
        }

        public override string ToString()
        {
            if (_value == null)
            {
                return null;
            }

            return _value.Select(a => a.ToString()).Aggregate((a, b) => a + ", " + b);
        }
    }

    public class InternalLogger
    {
        internal static NLog.Logger _Logger = NLog.LogManager.GetCurrentClassLogger();

        public bool IsDebugEnabled
        {
            get; set;
        }

        private InternalLogger()
        {
        }

        public static InternalLogger Create()
        {
            return new InternalLogger();
        }

        //public static string _func_([CallerMemberName] string funcName = "")
        //{
        //    return funcName;
        //}

        protected string ToString(params Stringable[] args)
        {
            return args.Select(a => '[' + a.ToString() + ']').Aggregate((a, b) => a + ", " + b);
        }


        public void Debug(string message, params Stringable[] args)
        {
            if (this.IsDebugEnabled)
            {
                _Logger.Debug($"{message}: {ToString(args)}");
            }
        }

        public void Info(string message, params Stringable[] args)
        {
            _Logger.Info($"{message}: {ToString(args)}");
        }

        public void Error(string message, params Stringable[] args)
        {
            _Logger.Error($"{message}: {ToString(args)}");
        }
    }
}
