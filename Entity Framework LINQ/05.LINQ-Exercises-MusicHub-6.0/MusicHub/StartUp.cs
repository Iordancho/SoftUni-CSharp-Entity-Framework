﻿using System;
using System.Linq;
using System.Text;
using MusicHub.Data;
using MusicHub.Data.Models;
using MusicHub.Initializer;

namespace MusicHub
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            MusicHubDbContext context = new MusicHubDbContext();
            DbInitializer.ResetDatabase(context);

           // Console.WriteLine(ExportAlbumsInfo(context, 9));
           Console.WriteLine(ExportSongsAboveDuration(context, 4));
        }

        // 2. All Albums Produced By Given Producer
        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albumsInfo = context.Producers
                .FirstOrDefault(x => x.Id == producerId)
                .Albums
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate,
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs.Select(s => new
                        {
                            SongName = s.Name,
                            SongPrice = s.Price,
                            SongWriterName = s.Writer.Name
                        })
                        .OrderByDescending(s => s.SongName)
                        .ThenBy(s => s.SongWriterName),
                    AlbumPrice = a.Price
                }).OrderByDescending(a => a.AlbumPrice)
                .ToList();

            var sb = new StringBuilder();

            foreach (var album in albumsInfo)
            {
                sb.AppendLine($"-AlbumName: {album.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate.ToString("MM/dd/yyyy")}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");

                int counter = 1;

                foreach (var song in album.Songs)
                {
                    sb.AppendLine($"---#{counter++}");
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.SongPrice:f2}");
                    sb.AppendLine($"---Writer: {song.SongWriterName}");
                }

                sb.AppendLine($"-AlbumPrice: {album.AlbumPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        
        // 3. 3.Songs Above Given Duration
        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            var span = new TimeSpan(0, 0, duration);
                       var songsAboveDuration = context
                           .Songs
                           .Where(s => s.Duration > span)
                           .Select(s => new
                           {
                               SongName = s.Name,
                               PerformerFullName = s.SongPerformers
                                   .Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName)
                                   .OrderBy(name => name)
                                   .ToList(),
                               WriterName = s.Writer.Name,
                               AlbumProducerName = s.Album.Producer.Name,
                               Duration = s.Duration.ToString("c")
                           })
                           .OrderBy(s => s.SongName)
                           .ThenBy(s => s.WriterName)
                           .ToList();
           
                       StringBuilder sb = new StringBuilder();
                       int counter = 1;
           
                       foreach (var s in songsAboveDuration)
                       {
                           sb
                               .AppendLine($"-Song #{counter++}")
                               .AppendLine($"---SongName: {s.SongName}")
                               .AppendLine($"---Writer: {s.WriterName}");
           
                           if (s.PerformerFullName.Any())
                           {
                               sb.AppendLine(string
                                   .Join(Environment.NewLine, s.PerformerFullName
                                       .Select(p => $"---Performer: {p}")));
                           }
           
                           sb
                               .AppendLine($"---AlbumProducer: {s.AlbumProducerName}")
                               .AppendLine($"---Duration: {s.Duration}");
                       }
           
                       return sb.ToString().TrimEnd();
        }
    }
}