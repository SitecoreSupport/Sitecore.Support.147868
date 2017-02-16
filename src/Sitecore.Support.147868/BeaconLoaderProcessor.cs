namespace Sitecore.Support.FXM
{
    using Sitecore.Abstractions;
    using Sitecore.Configuration;
    using Sitecore.Diagnostics;
    using Sitecore.FXM.Abstractions;
    using Sitecore.FXM.Pipelines.Bundle;
    using Sitecore.Services.Infrastructure.Sitecore;
    using Sitecore.StringExtensions;
    using System;
    using System.Web.Optimization;
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
            string setting = this.settings.GetSetting(Globals.ConfigurationSettings.Routes.BaseKey, Globals.ConfigurationSettings.Routes.BaseDefaultValue);
            string host = args.Context.Request.Url.Host;
            string str3 = "{0}{1}/{2}/{3}".FormatWith(new object[] { this.protocol, host.TrimEnd(new char[] { '/' }), setting.TrimEnd(new char[] { '/' }), this.service });
            string uniqueFilename = this.fileUtil.GetUniqueFilename(Settings.TempFolderPath + "/beacon.js");
            string text = "SCBeacon = new SCBeacon(\"" + str3 + "\");";
            Log.Audit(string.Concat(new object[] { "Sitecore.Support.FXM: args.Context.Request.Url: ", args.Context.Request.Url, " host: ", host, "; SCBeacon Url: ", str3 }), base.GetType());
            this.fileUtil.WriteToFile(uniqueFilename, text, false);
            args.Bundle.Include("~" + uniqueFilename, new IItemTransform[0]);
        }
    }
}
