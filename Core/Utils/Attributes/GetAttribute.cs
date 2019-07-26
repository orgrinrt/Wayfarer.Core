using System;

namespace Wayfarer.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GetAttribute : Attribute
    {
        public string NodePath { get; set; }
        
        public string NodePathSourceFieldName { get; set; }

        public bool IsFieldNameThatHasPath { get; set; }
        
        public GetAttribute(string path, bool isFieldNameThatHasPathNotActualPathString = false)
        {
            IsFieldNameThatHasPath = isFieldNameThatHasPathNotActualPathString;
            
            if (!IsFieldNameThatHasPath)
            {
                NodePath = path;
            }
            else
            {
                NodePathSourceFieldName = path;
            }
        }
        
    }
}