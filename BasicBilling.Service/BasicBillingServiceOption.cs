namespace BasicBilling.Service
{
    public class BasicBillingServiceOption
    {
        internal bool InMemoryDatabase { get; set; }
        public string ConnectionString { get; set; }

        public BasicBillingServiceOption UseDatabaseConnectionString(string value)
        {
            ConnectionString = value;
            return this;
        }

        public BasicBillingServiceOption UseInMemoryDatabase(bool value)
        {
            InMemoryDatabase = value;
            return this;
        }
    }
}
