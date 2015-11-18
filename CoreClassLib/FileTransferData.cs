using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace KynetLib
{
    [MessageContract]
    public class FileTransfer
    {
        public FileTransfer()
        {
            StartTime = DateTime.Now;
            ID = Guid.NewGuid();
        }

        //File and sender info
        [MessageHeader]
        public string Fingerprint { get; set; }
        [MessageHeader]
        public Guid ID { get; set; }
        [MessageHeader]
        public string FileName { get; set; }
        [MessageHeader]
        public string ClientFilePath { get; set; }
        [MessageHeader]
        public string ServerFilePath { get; set; }
        [MessageHeader]
        public long FileSize { get; set; }
        [MessageHeader]
        public TransferType transferType { get; set; }
        [MessageHeader]
        public string Error { get; set; }


        [MessageBodyMember]
        public Stream Data { get; set; }

        //Time info
        [MessageHeader]
        public DateTime StartTime { get; set; }
        public DateTime FinishTime
        {
            get
            {
                if (Completed)
                    return StartTime.Add(Duration);
                else
                    return DateTime.MaxValue;
            }
        }
        [MessageHeader]
        public TimeSpan Duration { get; set; }
        
        //Status info
        public float BytesRead
        {
            get
            {
                if (Data.CanRead)
                    return Data.Position;
                else
                    return -1;
            }
        }
        public string FileExtension
        {
            get
            {
                return Path.GetExtension(FileName);
            }
        }
        public float Progress
        {
            get
            {
                return ((BytesRead / FileSize) * 100f);
            }
        }

        //Completion info
        public bool Completed
        {
            get
            {
                return BytesRead == FileSize && BytesRead + FileSize > 0 ? true : false;
            }
        }
        public bool Failed
        {
            get
            {
                return Error != null ? true : false;
            }
        }

        public enum TransferType
        {
            Upload,
            Download
        }
    }

    [DataContract(Namespace = "Kynet")]
    public class DirectoryInformation
    {
        //File and sender info
        [DataMember]
        public string Fingerprint { get; set; }
        [DataMember]
        public string Folder { get; set; }
        [DataMember]
        public bool Failed = false;
        [DataMember]
        public List<FileData> Files { get; set; }
        [DataMember]
        public string[] Folders { get; set; }
    }

    [DataContract(Namespace = "Kynet")]
    public class FileData
    {
        [DataMember]
        public string Filename { get; set; }
        [DataMember]
        public string FileType { get; set; }
        [DataMember]
        public long Filesize { get; set; }
        [DataMember]
        public DateTime DateCreated { get; set; }
        [DataMember]
        public DateTime DateModified { get; set; }

        public static string GetFileType(string file)
        {
            return MimeMapping.GetMimeMapping(file).ToString();
        }
    }
}
