// Copyright (c) Six Labors and contributors.
// Licensed under the GNU Affero General Public License, Version 3.

using System.Collections.Generic;

namespace SixLabors.ImageSharp.Web.Providers.Azure
{
    /// <summary>
    /// Configuration options for the <see cref="AzureBlobStorageImageProvider"/> provider.
    /// </summary>
    public class AzureBlobStorageImageProviderOptions
    {
        /// <summary>
        /// Gets or sets the collection of blob container client options.
        /// </summary>
        public ICollection<AzureBlobContainerClientOptions> BlobContainers { get; set; } = new HashSet<AzureBlobContainerClientOptions>();
    }

    /// <summary>
    /// Represents a single Azure Blob Storage connection and container.
    /// </summary>
    public class AzureBlobContainerClientOptions
    {
        /// <summary>
        /// Gets or sets the Azure Blob Storage connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the Azure Blob Storage container name.
        /// Must conform to Azure Blob Storage containiner naming guidlines.
        /// <see href="https://docs.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata#container-names"/>
        /// </summary>
        public string ContainerName { get; set; }
    }
}
