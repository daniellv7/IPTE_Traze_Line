using Ipte.TS1.UI.Controls;
using IPTE_Base_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IPTE_Base_Project.ViewModels
{
   public class ValidationBaseViewModel : BaseViewModel, IDataErrorInfo
    {
        private Dictionary<string, Binder> ruleMap = new Dictionary<string, Binder>();
        
        public void AddRule<T>(string expression, Func<bool> ruleDelegate, string errorMessage)
        {
            ruleMap.Add(expression, new Binder(ruleDelegate, errorMessage));
        }

        public void InitValidationRules<T>(ValidationsModel model, string className)
        {
            if (model.validationList == null) return;
            foreach (Validation v in model.validationList.Validations)
            {
                if (v.ClassName.Equals(className))
                {
                    if (v.PropertyType.Equals("String"))
                    {
                        AddRule<string>(v.PropertyName,
                            () => checkString<T>(className, v.PropertyName, v.Min, v.Max, v.Regex),
                            v.ErrorMessage);
                    }
                    else if (v.PropertyType.Equals("Number"))
                    {
                        AddRule<string>(v.PropertyName,
                            () => checkNumber<T>(className, v.PropertyName, v.Min, v.Max, v.Regex),
                            v.ErrorMessage);
                    }
                }
            }
        }

        public bool checkString<T>(string className, string propertyName, double min, double max, string expression)
        {            
            System.Reflection.PropertyInfo prop = typeof(T).GetProperty(propertyName);
            string value = (string)prop.GetValue(this, null);

            bool result = value.Length >= (min) &&
                          value.Length <= (max) &&
                          Regex.Match(value, expression).Success;
            return result;
        }

        public bool checkNumber<T>(string className, string propertyName, double min, double max, string expression)
        {
            System.Reflection.PropertyInfo prop = typeof(T).GetProperty(propertyName);
            double value = (double)prop.GetValue(this, null);

            bool result = value >= (min) &&
                          value <= (max) &&
                          Regex.Match(value.ToString(), expression).Success;
            return result;
        }

        protected static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            Expression body = expression.Body;
            MemberExpression memberExpression = body as MemberExpression;
            if (memberExpression == null)
            {
                memberExpression = (MemberExpression)((UnaryExpression)body).Operand;
            }
            return memberExpression.Member.Name;
        }

        protected override void SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (ruleMap.ContainsKey(propertyName))
            {
                ruleMap[propertyName].IsDirty = true;
            }            
            base.SetValue<T>(value, propertyName);
        }

        public bool HasErrors
        {
            get
            {
                var values = ruleMap.Values.ToList();
                values.ForEach(b => b.Update());

                return values.Any(b => b.HasError);
            }
        }

        public string Error
        {
            get
            {
                var errors = from b in ruleMap.Values where b.HasError select b.Error;

                return string.Join("\n", errors);
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (ruleMap.ContainsKey(columnName))
                {
                    ruleMap[columnName].Update();
                    return ruleMap[columnName].Error;
                }
                return null;
            }
        }
        
        private class Binder
        {
            private readonly Func<bool> ruleDelegate;
            private readonly string message;

            internal Binder(Func<bool> ruleDelegate, string message)
            {
                this.ruleDelegate = ruleDelegate;
                this.message = message;

                IsDirty = true;
            }

            internal string Error { get; set; }
            internal bool HasError { get; set; }

            internal bool IsDirty { get; set; }

            internal void Update()
            {
                if (!IsDirty)
                    return;

                Error = null;
                HasError = false;
                try
                {
                    if (!ruleDelegate())
                    {
                        Error = message;
                        HasError = true;
                    }
                }
                catch (Exception e)
                {
                    Error = e.Message;
                    HasError = true;
                }
            }
        }
    }
}
