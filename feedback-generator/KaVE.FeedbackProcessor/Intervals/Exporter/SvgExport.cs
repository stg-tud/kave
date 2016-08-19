/*
 * Copyright 2014 Technische Universität Darmstadt
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using KaVE.Commons.Model.Events.TestRunEvents;
using KaVE.FeedbackProcessor.Intervals.Model;
using Svg;

namespace KaVE.FeedbackProcessor.Intervals.Exporter
{
    public class SvgExport
    {
        private const int LabelWidth = 170;
        private const int BarWidth = 1000;
        private const int MaxWidth = LabelWidth + BarWidth;
        private DateTime earliest;
        private DateTime latest;

        private SvgDocument _doc;

        public void Run(IList<Interval> intervals, string fileName)
        {
            SetBoundaries(intervals);

            _doc = new SvgDocument
            {
                Width = MaxWidth,
                Height = 300
            };

            var rows = new Dictionary<string, int>();
            var nextRow = 10;
            rows.Add("VisualStudioOpened", nextRow);
            nextRow += 15;
            rows.Add("Perspective", nextRow);
            nextRow += 15;
            rows.Add("UserActive", nextRow);
            nextRow += 15;

            foreach (var label in rows.Keys)
            {
                _doc.Nodes.Add(Label(rows[label], label));
            }

            var labelRows = new Dictionary<string, int>();
            foreach (var i in intervals)
            {
                var vsOpen = i as VisualStudioOpenedInterval;
                var pers = i as PerspectiveInterval;
                var ua = i as UserActiveInterval;
                var fi = i as FileInteractionInterval;
                var tr = i as TestRunInterval;


                var coords = GetCoords(i);
                var label = GetLabel(i);

                if (vsOpen != null)
                {
                    AddLine(5, coords, Color.DarkGray);
                }
                else if (pers != null)
                {
                    AddLine(
                        20,
                        coords,
                        pers.Perspective == PerspectiveType.Production ? Color.DodgerBlue : Color.DarkOrange);
                }
                else if (ua != null)
                {
                    AddLine(35, coords, Color.SaddleBrown);
                }
                else if (fi != null)
                {
                    var row = nextRow;

                    if (labelRows.ContainsKey(label))
                    {
                        row = labelRows[label];
                    }
                    else
                    {
                        labelRows[label] = row;
                        nextRow += 15;
                    }
                    var color = fi.Type == FileInteractionType.Reading ? Color.DarkGray : Color.Black;

                    _doc.Nodes.Add(Label(row, label));
                    AddLine(row, coords, color);
                }
                else if (tr != null)
                {
                    var strokeSize = 6;
                    var spacing = 3;

                    var row = nextRow;
                    var tcRow = row + strokeSize + spacing;
                    var mRow = tcRow + strokeSize + spacing;
                    nextRow = mRow + 15;

                    _doc.Nodes.Add(Label(row, label));
                    AddLine(row, coords, GetTestColor(tr.Result), 5);

                    var lastClassEnd = tr.StartTime;
                    var lastMethodEnd = tr.StartTime;
                    foreach (var tc in tr.TestClasses)
                    {
                        var tCoords = GetCoords(lastClassEnd, lastClassEnd + tc.Duration);
                        lastClassEnd += tc.Duration;
                        AddLine(tcRow, tCoords, GetTestColor(tc.Result), 5);

                        foreach (var tm in tc.TestMethods)
                        {
                            var mCoords = GetCoords(lastMethodEnd, lastMethodEnd + tm.Duration);
                            lastMethodEnd += tm.Duration;
                            AddLine(mRow, mCoords, GetTestColor(tm.Result), 5);
                        }
                    }
                }
                else
                {
                    _doc.Nodes.Add(Label(nextRow, label));
                    AddLine(nextRow, coords, Color.Black);
                    nextRow += 15;
                }
            }

            _doc.Write(fileName);
        }

        private static Color GetTestColor(TestResult tr)
        {
            return tr == TestResult.Error ? Color.Red : tr == TestResult.Success ? Color.Green : Color.LightGray;
        }

        private static SvgText Label(int row, string label)
        {
            return new SvgText
            {
                X = {LabelWidth - 10},
                Y = {row + 3},
                TextAnchor = SvgTextAnchor.End,
                FontSize = 10,
                FontFamily = "Arial",
                Nodes = {new SvgContentNode {Content = label}}
            };
        }

        private static string GetLabel(Interval interval)
        {
            var fi = interval as FileInteractionInterval;
            if (fi != null)
            {
                const int maxlen = 30;
                var n = fi.FileName;
                if (n.Length < maxlen)
                {
                    return "(file) " + n;
                }
                return "(file) ..." + n.Substring(n.Length - (maxlen - 3));
            }
            return interval.GetType().Name.Replace("Interval", "");
        }

        private TimeCoordinates GetCoords(Interval interval)
        {
            return GetCoords(interval.StartTime, interval.EndTime);
        }

        private TimeCoordinates GetCoords(DateTime startdt, DateTime enddt)
        {
            var wholeWidth = (latest - earliest).TotalSeconds;
            var start = BarWidth*(startdt - earliest).TotalSeconds/wholeWidth;
            var end = BarWidth*(enddt - earliest).TotalSeconds/wholeWidth;
            return new TimeCoordinates {Start = LabelWidth + (float) start, End = LabelWidth + (float) end};
        }

        private class TimeCoordinates
        {
            public float Start { get; set; }
            public float End { get; set; }
        }

        private void SetBoundaries(IEnumerable<Interval> intervals)
        {
            earliest = DateTime.MaxValue;
            latest = DateTime.MinValue;

            foreach (var i in intervals)
            {
                if (i.StartTime < earliest)
                {
                    earliest = i.StartTime;
                }
                if (i.EndTime > latest)
                {
                    latest = i.EndTime;
                }
            }
        }

        private void AddLine(int y, TimeCoordinates coords, Color color, float strokeWidth = 10)
        {
            var x1 = new SvgUnit(coords.Start);
            var x2 = new SvgUnit(coords.End)-0.1f;

            var mWidth = 0.5f;
            var m1x = new SvgUnit(x1 + mWidth/2);
            var m2x = new SvgUnit(x2 - mWidth/2);
            _doc.Nodes.Add(
                new SvgLine
                {
                    StartX = m1x,
                    EndX = m1x,
                    StartY = y,
                    EndY = new SvgUnit(y - strokeWidth/2 - 1f),
                    StrokeWidth = new SvgUnit(mWidth),
                    Stroke = new SvgColourServer(color)
                });
            _doc.Nodes.Add(
                new SvgLine
                {
                    StartX = x1,
                    EndX = x2,
                    StartY = y,
                    EndY = y,
                    StrokeWidth = strokeWidth,
                    Stroke = new SvgColourServer(color)
                });
            _doc.Nodes.Add(
                new SvgLine
                {
                    StartX = m2x,
                    EndX = m2x,
                    StartY = y,
                    EndY = new SvgUnit(y + strokeWidth/2 + 1f),
                    StrokeWidth = new SvgUnit(mWidth),
                    Stroke = new SvgColourServer(color)
                });
        }
    }
}