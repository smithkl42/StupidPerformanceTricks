using System;
using System.Collections.Generic;
using System.Reflection;

namespace StupidPerformanceTricks
{
	class ShallowCloneTest
	{
		private static int iterations = 50000;

		private static readonly PerformanceMonitor referenceCopy = new PerformanceMonitor("ReferenceCopy");
		private static readonly PerformanceMonitor shallowCloneNoCache = new PerformanceMonitor("ShallowCloneNoCache");
		private static readonly PerformanceMonitor shallowCloneWithCache = new PerformanceMonitor("ShallowCloneWithCache");

	    public static Organization _organization = new Organization();
	    public static Organization _copy;

		public static void Test()
		{
            TestHelper.PerformTest(iterations, referenceCopy, ReferenceCopy);
			TestHelper.PerformTest(iterations, shallowCloneNoCache, ShallowCloneNoCache);
			TestHelper.PerformTest(iterations, shallowCloneWithCache, ShallowCloneCache);
		}

		private static void ReferenceCopy(int i)
		{
		    _copy = _organization;
		}

		private static void ShallowCloneNoCache(int i)
		{
		    _copy = _organization.ShallowClone();
		}

		private static void ShallowCloneCache(int i)
		{
		    _copy = _organization.ShallowCloneWithCache();
		}
	}

    public static class TypeHelper
    {
        public static T ShallowClone<T>(this T obj) where T : class
        {
            if (obj == null) return null;
            var newObj = Activator.CreateInstance<T>();
            var fields = typeof(T).GetFields();
            foreach (var field in fields)
            {
                if (field.IsPublic && (field.FieldType.IsValueType || field.FieldType == typeof(string)))
                {
                    field.SetValue(newObj, field.GetValue(obj));
                }
            }
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if ((property.CanRead && property.CanWrite) &&
                    (property.PropertyType.IsValueType || property.PropertyType == typeof(string)))
                {
                    property.SetValue(newObj, property.GetValue(obj, null), null);
                }
            }
            return newObj;
        }

        private static readonly Dictionary<Type,FieldInfo[]> fieldInfoCache = new Dictionary<Type, FieldInfo[]>();
        private static readonly Dictionary<Type,PropertyInfo[]> propertyInfoCache = new Dictionary<Type, PropertyInfo[]>();

        public static T ShallowCloneWithCache<T>(this T obj) where T : class
        {
            if (obj == null) return null;
            var newObj = Activator.CreateInstance<T>();
            FieldInfo[] fields;
            var type = typeof (T);
            if (!fieldInfoCache.TryGetValue(type, out fields))
            {
                fields = type.GetFields();
                fieldInfoCache[type] = fields;
            }            
            foreach (var field in fields)
            {
                if (field.IsPublic && (field.FieldType.IsValueType || field.FieldType == typeof(string)))
                {
                    field.SetValue(newObj, field.GetValue(obj));
                }
            }
            PropertyInfo[] properties;
            if (!propertyInfoCache.TryGetValue(type, out properties))
            {
                properties = type.GetProperties();
                fieldInfoCache[type] = fields;
            }            

            foreach (var property in properties)
            {
                if ((property.CanRead && property.CanWrite) &&
                    (property.PropertyType.IsValueType || property.PropertyType == typeof(string)))
                {
                    property.SetValue(newObj, property.GetValue(obj, null), null);
                }
            }
            return newObj;
        }
    }

        public partial class Organization
    {
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string ZendeskLocation { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Nullable<DateTime> DeletedOn { get; set; }
        public string StripeCustomerId { get; set; }
        public Nullable<int> PlanId { get; set; }
        public string StripeChargeId { get; set; }
        public string StripeCardId { get; set; }
        public string CardLast4 { get; set; }
        public Nullable<int> CardExpirationMonth { get; set; }
        public Nullable<int> CardExpirationYear { get; set; }
        public string CardType { get; set; }
        public string CardCountry { get; set; }
        public Guid ApiKey { get; set; }
        public string PrivateApiKey { get; set; }
        public Nullable<DateTime> LastSynchronizedOn { get; set; }
        public string WidgetMonkeypatch { get; set; }
        public bool UseWidgetMonkeypatch { get; set; }
        public string LastSyncToken { get; set; }
        public bool PauseDataCollection { get; set; }
    
        public virtual object Country { get; set; }
        public virtual ICollection<object> Customers { get; set; }
        public virtual ICollection<object> OAuthTokens { get; set; }
        public virtual ICollection<object> StripeCustomers { get; set; }
        public virtual ICollection<object> ThirdPartyCredentials { get; set; }
        public virtual ICollection<object> TwitterUsers { get; set; }
        public virtual ICollection<object> Users { get; set; }
        public virtual ICollection<object> ZendeskOrganizations { get; set; }
        public virtual ICollection<object> ZendeskUsers { get; set; }
        public virtual ICollection<object> Campaigns { get; set; }
        public virtual object Plan { get; set; }
        public virtual ICollection<object> OAuth2Token { get; set; }
        public virtual ICollection<object> OrganizationEvents { get; set; }
        public virtual ICollection<object> CustomerUserAttributeKeys { get; set; }
        public virtual ICollection<object> OrganizationSelectors { get; set; }
        public virtual object KissMetricsConfiguration { get; set; }
        public virtual ICollection<object> OrganizationCookies { get; set; }
        public virtual object MixpanelConfiguration { get; set; }
    }

}
