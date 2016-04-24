﻿using System;
using System.Threading.Tasks;
using Voat.Data;
using Voat.Models;

namespace Voat.Domain.Command
{
    public class CommentVoteCommand : VoteCommand
    {
        public CommentVoteCommand(int commentID, int voteValue, bool revokeOnRevote = true) : base(voteValue)
        {
            CommentID = commentID;
            RevokeOnRevote = revokeOnRevote;
        }

        public int CommentID { get; private set; }

        protected override async Task<Tuple<VoteResponse, VoteResponse>> ProtectedExecute()
        {
            using (var db = new Repository())
            {
                var outcome = await Task.Run(() => db.VoteComment(CommentID, VoteValue, RevokeOnRevote));
                return new Tuple<VoteResponse, VoteResponse>(outcome, outcome);
            }
        }

        protected override void UpdateCache(VoteResponse result)
        {
            if (result.Successfull)
            {
                //update cache somehow
            }
        }
    }

    public class SubmissionVoteCommand : VoteCommand
    {
        public SubmissionVoteCommand(int submissionID, int voteValue, bool revokeOnRevote = true) : base(voteValue)
        {
            SubmissionID = submissionID;
            RevokeOnRevote = revokeOnRevote;
        }

        public int SubmissionID { get; private set; }

        protected override async Task<Tuple<VoteResponse, VoteResponse>> ProtectedExecute()
        {
            using (var gateway = new Repository())
            {
                var outcome = await Task.Factory.StartNew(() => gateway.VoteSubmission(SubmissionID, VoteValue, RevokeOnRevote));
                return new Tuple<VoteResponse, VoteResponse>(outcome, outcome);
            }
        }

        protected override void UpdateCache(VoteResponse result)
        {
            if (result.Successfull)
            {
                //update cache somehow
            }
        }
    }

    public abstract class VoteCommand : CacheCommand<VoteResponse, VoteResponse>
    {
        public VoteCommand(int voteValue, bool revokeOnRevote = true)
        {
            if (voteValue < -1 || voteValue > 1)
            {
                throw new ArgumentOutOfRangeException("voteValue", voteValue, "Invalid vote value");
            }
            this.VoteValue = voteValue;
            this.RevokeOnRevote = revokeOnRevote;
        }

        public bool RevokeOnRevote { get; protected set; }
        public int VoteValue { get; private set; }
    }
}