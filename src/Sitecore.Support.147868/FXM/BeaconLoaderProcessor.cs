using Sitecore.Abstractions;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.FXM.Abstractions;
using Sitecore.FXM.Pipelines.Bundle;
using Sitecore.FXM.Utilities;
using Sitecore.StringExtensions;
using System;
using System.Web.Optimization;

namespace Sitecore.Support.FXM
{
    public class BeaconLoaderProcessor : IBundleProcessor<OnRequestBundleGeneratorArgs>
    {
        private readonly IFileUtil fileUtil;
        private readonly string protocol;
        private readonly string service;
        private readonly ISettings settings;

        public BeaconLoaderProcessor() : this(new SettingsWrapper(), new FileUtilWrapper())
        {
        }

        public BeaconLoaderProcessor(ISettings settings, IFileUtil fileUtil)
        {
            this.settings = settings;
            this.fileUtil = fileUtil;
            this.protocol = this.settings.GetSetting("FXM.Protocol", string.Empty);
            this.service = "Beacon.Service".Replace('.', '/');
        }

        public void Process(OnRequestBundleGeneratorArgs args)
        {
            string setting = this.settings.GetSetting("Sitecore.Services.RouteBase", "sitecore/api/ssc/");
            string uriHost = FxmUtility.GetUriHost(args.Context.Request.Url);
            object[] parameters = new object[4];
            parameters[0] = this.protocol;
            char[] trimChars = new char[] { '/' };
            parameters[1] = uriHost.TrimEnd(trimChars);
            char[] chArray2 = new char[] { '/' };
            parameters[2] = setting.TrimEnd(chArray2);
            parameters[3] = this.service;
            string str3 = "{0}{1}/{2}/{3}".FormatWith(parameters);
            string uniqueFilename = this.fileUtil.GetUniqueFilename(Settings.TempFolderPath + "/beacon.js");
            string text = "SCBeacon = new SCBeacon(\"" + str3 + "\");";
            this.fileUtil.WriteToFile(uniqueFilename, text, false);
            args.Bundle.Include("~" + uniqueFilename, new IItemTransform[0]);
        }
    }
}
