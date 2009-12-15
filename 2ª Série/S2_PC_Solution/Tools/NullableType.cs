namespace Tools
{
    struct NullableType<T>
    {
        T value;
        bool hasValue;

        public T Value
        {
            get { return value; }
            set { hasValue = true; this.value = value; }
        }
        public bool HasValue
        {
            get { return hasValue; }
        }
    }
}
