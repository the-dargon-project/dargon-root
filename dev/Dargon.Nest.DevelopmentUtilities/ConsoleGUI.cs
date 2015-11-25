using System;

namespace Dargon.Nest.DevelopmentUtilities {
   public class CliProgressSpinner {
      private readonly string taskName;
      private readonly int x;
      private readonly int y;
      private string status = null;

      public CliProgressSpinner(string taskName) {
         this.taskName = taskName;

         if (Console.CursorLeft != 0) {
            Console.WriteLine();
         }
         this.x = Console.CursorLeft;
         this.y = Console.CursorTop;
         Console.WriteLine();
      }

      public void Render() {
         using (new ConsolePosition(x, y)) {
            var w = Console.BufferWidth;
            string progress = status ?? ("-\\|/"[(DateTime.Now.Millisecond / 125) % 4] + "");
            if (taskName.Length + progress.Length > w) {
               var finalLength = Math.Max(0, w - taskName.Length);
               progress = progress.Substring(progress.Length - finalLength);
            }
            Console.Write(taskName.PadRight(w - progress.Length) + progress);
         }
      }

      public void Update() {
         status = null;
         Render();
      }

      public void Update(float percentage) {
         status = (int)Math.Round(percentage * 100) + "%";
         Render();
      }

      public void Update(string status) {
         this.status = status;
         Render();
      }
   }

   public class ConsolePosition : IDisposable {
      private int x;
      private int y;

      public ConsolePosition(int newX, int newY) {
         x = Console.CursorLeft;
         y = Console.CursorTop;
         Console.CursorLeft = newX;
         Console.CursorTop = newY;
      }

      public void Dispose() {
         Console.CursorLeft = x;
         Console.CursorTop = y;
      }
   }
}