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

using System.Text;
using KaVE.Commons.Model.Events.UserProfiles;

namespace KaVE.RS.SolutionAnalysis.UserProfileExports
{
    public static class UserProfilePrinter
    {
        public static string ToStringExt(this UserProfileEvent e)
        {
            var sb = new StringBuilder();
            sb.Open()
              .Field("ProfileId", Quote(e.ProfileId))
              .Field("Education", e.Education)
              .Field("Position", e.Position)
              .Field("date", e.TriggeredAt.HasValue ? e.TriggeredAt.ToString() : "-")
              .Close();
            return sb.ToString();
        }

        private static string Quote(object o)
        {
            return string.Format("\"{0}\"", o);
        }

        public static StringBuilder Open(this StringBuilder sb)
        {
            sb.Append("{\n");
            return sb;
        }

        public static StringBuilder Field(this StringBuilder sb, string name, object content)
        {
            sb.Append('\t').Append(name).Append(": ").Append(content).Append('\n');
            return sb;
        }

        public static StringBuilder Close(this StringBuilder sb)
        {
            sb.Append("}");
            return sb;
        }
    }
}