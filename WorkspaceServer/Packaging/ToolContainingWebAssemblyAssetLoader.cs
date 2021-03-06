// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkspaceServer.WorkspaceFeatures;

namespace WorkspaceServer.Packaging
{
    public class ToolContainingWebAssemblyAssetLoader : IPackageAssetLoader
    {
        private readonly IToolPackageLocator _toolPackageLocator;

        public ToolContainingWebAssemblyAssetLoader(IToolPackageLocator toolPackageLocator = null)
        {
            _toolPackageLocator = toolPackageLocator ??
                                  new ToolPackageLocator();
        }

        public async Task<IEnumerable<PackageAsset>> LoadAsync(Package2 package)
        {
            var directory = package.DirectoryAccessor;

            if (directory.DirectoryExists(".store"))
            {
                var exeName = package.Name.ExecutableName();

                if (directory.FileExists(exeName))
                {
                    var tool = new PackageTool(package.Name, directory.GetFullyQualifiedRoot());
                    var exePath = directory.GetFullyQualifiedFilePath(exeName);

                    var toolDirectory = await _toolPackageLocator.PrepareToolAndLocateAssetDirectory(tool);

                    return new PackageAsset[]
                           {
                               new WebAssemblyAsset(directory.GetDirectoryAccessorFor(toolDirectory))
                           };
                }
            }

            return Enumerable.Empty<PackageAsset>();
        }
    }
}