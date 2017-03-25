using System.Reflection;

namespace BloodOfEvil.Extensions
{
    public static class MemberInfoExtension
    {
        /// <summary>
        /// Récupère des informations sur un MemberInfo.
        /// </summary>
        public static string GetMemberInfoMoreInformations(this MemberInfo memberInfo)
        {
            return string.Format("name : {0}, type : {1}, memberType : {2}",
                memberInfo.Name,
                memberInfo.GetType(),
                memberInfo.MemberType);
        }
    }
}