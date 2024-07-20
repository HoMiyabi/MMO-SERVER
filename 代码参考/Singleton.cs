using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	//where T : class代表对T的类型约束,即T必须为class类或者class类的子类
	//where T: new() 代表该类型必须具有无参数构造函数。有了这个约束将允许这样初始化T类型的实例
	public class Singleton<T> where T : new()
	{
		//加问号代表可null类型
		private static T? instance;
		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new T();
				}
				return instance;
			}
		}
	}
}
