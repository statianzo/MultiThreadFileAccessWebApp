using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace MultithreadedFileAccessWebApp.Framework
{
	public class QueuedWriter
	{
		private static readonly Queue RootQueue = new Queue();
		private static readonly string StorageDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "storage");
		private static readonly Queue SyncQueue = Queue.Synchronized(RootQueue);
		private static readonly IList<string> WrittenFiles = new List<string>();
		private static bool _running;

		public static IList<string> CompletedFiles
		{
			get { return new ReadOnlyCollection<string>(WrittenFiles); }
		}

		public static void Enqueue(string fileName)
		{
			SyncQueue.Enqueue(fileName);
		}

		private static string Dequeue()
		{
			return SyncQueue.Count > 0
			       	? SyncQueue.Dequeue() as string
			       	: null;
		}

		public static void StartWorker()
		{
			if (_running)
				return;
			_running = true;
			var backgroundWorker = new Thread(WorkerLoop) {IsBackground = true};
			backgroundWorker.Start();
		}

		public static void StopWorker()
		{
			_running = false;
		}

		private static void WorkerLoop()
		{
			while (_running)
			{
				string item = Dequeue();
				if (item == null)
				{
					Thread.Sleep(100);
					continue;
				}
				if (WrittenFiles.Contains(item))
					continue;

				string description = DataAccess.GetDescription(item);
				if (description == null)
					continue;
				string filePath = Path.Combine(StorageDir, item);
				using (FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
				using (var streamWriter = new StreamWriter(fileStream))
				{
					streamWriter.Write(description);
					Thread.Sleep(500); //Long running operation
				}
				WrittenFiles.Add(item);
			}
		}
	}
}