using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
namespace Reproductor
{
    class Delay : ISampleProvider
    {
        private ISampleProvider fuente;
        public int OffsetMilisegundos { get; set; }
        private int cantidadMuestrasOffset;

        private List<float> bufferDelay = new List<float>();

        int tamañoBuffer;
        private int duracionBufferSegundos;
        private int cantidadMuestrasTranscurridas = 0;
        private int cantidadMuestrasBorradas = 0;

        public WaveFormat WaveFormat
        {
            get
            {
                return fuente.WaveFormat;
            }
        }

        public Delay(ISampleProvider fuente)
        {
            this.fuente = fuente;
            OffsetMilisegundos = 500;
            cantidadMuestrasOffset = (int) ((float)OffsetMilisegundos / 1000.0f);
            duracionBufferSegundos = 10;
            tamañoBuffer = fuente.WaveFormat.SampleRate * duracionBufferSegundos;
        }


        public int Read(float[] buffer, int offset, int count)
        {
            //Leemos las muestras 
            var read =
                fuente.Read(buffer, offset, count);

            float tiempoTranscurridoSegundos = 
                (float)cantidadMuestrasTranscurridas /
                (float) fuente.WaveFormat.SampleRate;
            int milisegundosTranscurridos =
                (int)tiempoTranscurridoSegundos * 1000;

            //Llenar el Buffer
            for(int i = 0; i < read; i++)
            {

                bufferDelay.Add(buffer[i + offset]);

            }

            //Eliminar los excedentes del Buffer
            if(bufferDelay.Count > tamañoBuffer)
            {
                int diferencia = bufferDelay.Count - tamañoBuffer;
                bufferDelay.RemoveRange(0, diferencia);
            }

            //Aplicar el efecto
            if(milisegundosTranscurridos > OffsetMilisegundos)
            {
                for(int i = 0; i < read; i++)
                {
                    buffer[offset + i] +=
                    bufferDelay[cantidadMuestrasTranscurridas - cantidadMuestrasBorradas + i - cantidadMuestrasOffset];
                }
            }

            cantidadMuestrasTranscurridas += read;
            return read;
        }
    }

}
