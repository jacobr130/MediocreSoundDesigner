using MediocreSoundDesigner;
using System.Data;
using System.Security.Cryptography;

namespace SoundDesignerTest
{
    [TestClass]
    public sealed class UnitTest
    {
        private float[] samples = { 1, 2, 3, 4, 5 };
        private float[] reverse = { 5, 4, 3, 2, 1 };
        private Track track = null!;
        private TrackManager tm = null!;

        [TestInitialize]
        public void Setup() 
        { 
            track = new Track((float[])samples.Clone()); 
            tm = new TrackManager(null);
            var t1 = new Track((float[])samples.Clone());
            var t2 = new Track((float[])samples.Clone());
            var ctr1 = new CtrlTrackRow(t1);
            var ctr2 = new CtrlTrackRow(t2);
            tm.AddTrack(ctr1);
            tm.AddTrack(ctr2);
        }

        [TestCleanup]
        public void Cleanup() { track.Dispose(); tm.Dispose(); }

        /// Tests for Track.cs

        [TestMethod]
        public void Reverse_reverses()
        {
            track.Reverse();
            CollectionAssert.AreEqual(reverse, track.Samples);
        }

        [TestMethod]
        public void EliminateSilence_eliminatesSilence()
        {
            int silenceLen = track.SampleRate;
            var samplesWithSilence = new float[silenceLen * 2 + 2];
            samplesWithSilence[silenceLen] = 1;
            samplesWithSilence[silenceLen + 1] = 2;

            float minDuration = 0.1f;
            float threshold = 0.001f;

            // test each region
            track = new Track((float[])samplesWithSilence.Clone());
            float[] expected = { 1, 2 };
            track.EliminateSilence(SampleRegion.BEGINNING_AND_END, minDuration, threshold);
            CollectionAssert.AreEqual(expected, track.Samples);

            track = new Track((float[])samplesWithSilence.Clone());
            expected = new float[silenceLen + 2];
            expected[silenceLen] = 1;
            expected[silenceLen + 1] = 2;
            track.EliminateSilence(SampleRegion.END_ONLY, minDuration, threshold);
            CollectionAssert.AreEqual(expected, track.Samples);

            track = new Track((float[])samplesWithSilence.Clone());
            expected = new float[silenceLen + 2];
            expected[0] = 1;
            expected[1] = 2;
            track.EliminateSilence(SampleRegion.BEGINNING_ONLY, minDuration, threshold);
            CollectionAssert.AreEqual(expected, track.Samples);

            samplesWithSilence = new float[silenceLen * 2 + 2];
            samplesWithSilence[0] = 1;
            samplesWithSilence[silenceLen * 2  + 1] = 2;
            track = new Track((float[])samplesWithSilence.Clone());
            expected = [1, 2];
            track.EliminateSilence(SampleRegion.ANYWHERE, minDuration, threshold);
            CollectionAssert.AreEqual(expected, track.Samples);

            // check big numbers
            float bigMinDuration = float.MaxValue;
            float bigThreshold = float.MaxValue;

            track = new Track((float[])samplesWithSilence.Clone());
            track.EliminateSilence(SampleRegion.ANYWHERE, bigMinDuration, threshold);
            CollectionAssert.AreEqual(samplesWithSilence, track.Samples);

            track = new Track((float[])samplesWithSilence.Clone());
            track.EliminateSilence(SampleRegion.ANYWHERE, minDuration, bigThreshold);
            CollectionAssert.AreEqual(new float[0], track.Samples);

            track = new Track((float[])samplesWithSilence.Clone());
            track.EliminateSilence(SampleRegion.ANYWHERE, bigMinDuration, bigThreshold);
            CollectionAssert.AreEqual(samplesWithSilence, track.Samples);
        }

        [TestMethod]
        public void Flip_flips()
        {
            float[] expected = { -1, -2, -3, -4, -5 };
            track.Flip();
            CollectionAssert.AreEqual(expected, track.Samples);
        }

        [TestMethod]
        public void Undo_undoes()
        {
            // undo reverse
            track.Reverse();
            track.Undo();
            CollectionAssert.AreEqual(samples, track.Samples);

            // undo volume change
            track.SetVolume(0.5f);
            track.Undo();
            CollectionAssert.AreEqual(samples, track.Samples);
            Assert.AreEqual(1f, track.Volume);

            // undo flip
            track.Flip();
            track.Undo();
            CollectionAssert.AreEqual(samples, track.Samples);

            // undo reverb
            track.Reverb();
            track.Undo();
            CollectionAssert.AreEqual(samples, track.Samples);

            // undo eliminate silence
            float dur = 0.1f;
            float thresh = 0.001f;
            foreach (SampleRegion region in Enum.GetValues(typeof(SampleRegion)))
            {
                track.EliminateSilence(region, dur, thresh);
                track.Undo();
                CollectionAssert.AreEqual(samples, track.Samples);
            }
        }

        // tests for TrackManager.cs

        [TestMethod]
        public void SoloTrack_solosTrack()
        {
            var track1 = tm.TrackRows[0].track;
            var track2 = tm.TrackRows[1].track;

            tm.SoloTrack(track1);

            Assert.IsTrue(track1.IsSolo);
            Assert.IsFalse(track2.IsSolo);
            Assert.IsTrue(track2.IsDisabled);
        }

        [TestMethod]
        public void UnSoloTrack_unsolosTrack()
        {
            var track1 = tm.TrackRows[0].track;
            var track2 = tm.TrackRows[1].track;

            tm.SoloTrack(track1);
            tm.UnSoloTrack(track1);

            Assert.IsFalse(track1.IsSolo);
            Assert.IsFalse(track2.IsSolo);
            Assert.IsFalse(track1.IsDisabled);
            Assert.IsFalse(track2.IsDisabled);
        }
    }
}