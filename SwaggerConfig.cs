using XXX.XXX.WebApi;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace XXX.XXX.WebApi
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "WebApi Document");
                        // 取消失效方法
                        c.IgnoreObsoleteActions();

                        // 字段注释
                        c.IncludeXmlComments(GetXmlCommentsPath("XXX.XXX.WebApi"));
                        c.IncludeXmlComments(GetXmlCommentsPath("XXX.XXX.Model"));

                        // swagger展示
                        c.CustomProvider((defaultProvider) => new CachingSwaggerProvider(defaultProvider));

                        // 请求示例取消字段赋值
                        c.SchemaFilter<OmitIgnoredPropertiesModelFilter>();

                        // 添加header请求数据
                        c.OperationFilter<AddRequiredHeaderParameter>();

                    })
                .EnableSwaggerUi(c =>
                    {
                        // 汉化js
                        c.InjectJavaScript(thisAssembly, "XXX.XXX.WebApi.Scripts.swagger_lang.js");
                        c.EnableDiscoveryUrlSelector();
                    });
        }

        private static string GetXmlCommentsPath(string name)
        {
            //获取项目dll 所在的bin目录
            string xmlName = string.Format(@"{0}\bin\{1}.xml", AppDomain.CurrentDomain.BaseDirectory, name);
            return xmlName;
        }

        public class CachingSwaggerProvider : ISwaggerProvider
        {
            private static ConcurrentDictionary<string, SwaggerDocument> _cache =
                new ConcurrentDictionary<string, SwaggerDocument>();

            private readonly ISwaggerProvider _swaggerProvider;

            public CachingSwaggerProvider(ISwaggerProvider swaggerProvider)
            {
                _swaggerProvider = swaggerProvider;
            }

            public SwaggerDocument GetSwagger(string rootUrl, string apiVersion)
            {
                var cacheKey = string.Format("{0}_{1}", rootUrl, apiVersion);
                SwaggerDocument srcDoc = null;
                //只读取一次
                if (!_cache.TryGetValue(cacheKey, out srcDoc))
                {
                    srcDoc = _swaggerProvider.GetSwagger(rootUrl, apiVersion);

                    srcDoc.vendorExtensions = new Dictionary<string, object> { { "ControllerDesc", GetControllerDesc() } };
                    _cache.TryAdd(cacheKey, srcDoc);
                }
                return srcDoc;
            }

            public static ConcurrentDictionary<string, string> GetControllerDesc()
            {
                string xmlpath = string.Format("{0}/bin/XXX.XXX.WebApi.XML", AppDomain.CurrentDomain.BaseDirectory);
                ConcurrentDictionary<string, string> controllerDescDict = new ConcurrentDictionary<string, string>();
                if (File.Exists(xmlpath))
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(xmlpath);
                    string type = string.Empty, path = string.Empty, controllerName = string.Empty;

                    string[] arrPath;
                    int length = -1, cCount = "Controller".Length;
                    XmlNode summaryNode = null;
                    foreach (XmlNode node in xmldoc.SelectNodes("//member"))
                    {
                        type = node.Attributes["name"].Value;
                        if (type.StartsWith("T:"))
                        {
                            //控制器
                            arrPath = type.Split('.');
                            length = arrPath.Length;
                            controllerName = arrPath[length - 1];
                            if (controllerName.EndsWith("Controller"))
                            {
                                //获取控制器注释
                                summaryNode = node.SelectSingleNode("summary");
                                string key = controllerName.Remove(controllerName.Length - cCount, cCount);
                                if (summaryNode != null && !string.IsNullOrEmpty(summaryNode.InnerText) && !controllerDescDict.ContainsKey(key))
                                {
                                    controllerDescDict.TryAdd(key, summaryNode.InnerText.Trim());
                                }
                            }
                        }
                    }
                }
                return controllerDescDict;
            }
        }

        public class OmitIgnoredPropertiesModelFilter : ISchemaFilter
        {
            public void Apply(Schema schema, SchemaRegistry schemaRegistry, Type type)
            {
                #region 过滤请求参数示例中的agentKey和equip字段

                if (type.FullName.StartsWith("XXX.XXX.WebApi.Models"))
                {
                    var properties = type.GetProperties();

                    foreach (var p in properties)
                    {
                        string name = p.Name;

                        try
                        {
                            if (p.GetCustomAttributes(typeof(DataMemberAttribute), false).Length > 0)
                            {
                                name = ((DataMemberAttribute)p.GetCustomAttributes(typeof(DataMemberAttribute), false)[0]).Name;
                            }
                        }
                        catch
                        {
                            name = p.Name;
                        }

                        if (name.ToUpper() == "AGENTKEY" || name.ToUpper() == "EQUIP")
                        {
                            schema.properties.Remove(name);
                        }
                    }
                }

                #endregion
            }
        }

        public class AddRequiredHeaderParameter : IOperationFilter
        {
            public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
            {
                if (operation.parameters == null)
                {
                    operation.parameters = new List<Parameter>();
                }

                operation.parameters.Add(new Parameter()
                {
                    name = "agentKey",
                    @default = "test",
                    @in = "header",
                    type = "string",
                    description = "agent key",
                    required = true
                });

                operation.parameters.Add(new Parameter()
                {
                    name = "equip",
                    @default = "P",
                    @in = "header",
                    type = "string",
                    description = @"source from  I:iOS, A:Android, P:PC Broswer",
                    required = false
                });
            }
        }
    }
}
