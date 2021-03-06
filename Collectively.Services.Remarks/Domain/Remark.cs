﻿using System;
using System.Collections.Generic;
using System.Linq;
using  Collectively.Common.Domain;
using  Collectively.Common.Extensions;
using  Collectively.Common.Types;


namespace Collectively.Services.Remarks.Domain
{
    public class Remark : IdentifiableEntity, ITimestampable
    {
        private ISet<RemarkPhoto> _photos = new HashSet<RemarkPhoto>();
        private ISet<RemarkState> _states = new HashSet<RemarkState>();
        private ISet<string> _tags = new HashSet<string>();
        private ISet<Vote> _votes = new HashSet<Vote>();
        public RemarkUser Author { get; protected set; }
        public RemarkCategory Category { get; protected set; }
        public Location Location { get; protected set; }
        public int Rating { get; protected set; }
        public RemarkState State { get; protected set; }
        public string Description { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public bool Resolved => State?.State == RemarkState.Names.Resolved;

        public IEnumerable<RemarkPhoto> Photos
        {
            get { return _photos; }
            protected set { _photos = new HashSet<RemarkPhoto>(value); }
        }

        public IEnumerable<RemarkState> States
        {
            get { return _states; }
            protected set { _states = new HashSet<RemarkState>(value); }
        }

        public IEnumerable<string> Tags
        {
            get { return _tags; }
            protected set { _tags = new HashSet<string>(value); }
        }

        public IEnumerable<Vote> Votes
        {
            get { return _votes; }
            protected set { _votes = new HashSet<Vote>(value); }
        }

        protected Remark()
        {
        }

        public Remark(Guid id, User author, Category category, Location location,
            string description = null)
        {
            Id = id;
            SetAuthor(author);
            SetCategory(category);
            SetLocation(location);
            SetDescription(description);
            SetState(RemarkState.New(Author, location, description));
            CreatedAt = DateTime.UtcNow;
        }

        public void SetAuthor(User author)
        {
            if (author == null)
            {
                throw new DomainException(OperationCodes.RemarkAuthorNotProvided,
                    "Remark author can not be null.");
            }
            Author = RemarkUser.Create(author);
        }

        public void SetCategory(Category category)
        {
            if (category == null)
            {
                throw new DomainException(OperationCodes.RemarkCategoryNotProvided,
                    "Remark category can not be null.");
            }
            Category = RemarkCategory.Create(category);
        }

        public void SetLocation(Location location)
        {
            if (location == null)
            {
                throw new DomainException(OperationCodes.RemarkLocationNotProvided, 
                    "Remark location can not be null.");
            }
            Location = location;
        }

        public void AddPhoto(RemarkPhoto photo)
        {
            if (photo == null)
            {
                return;
            }
            _photos.Add(photo);
        }
        
        public Maybe<RemarkPhoto> GetPhoto(string name) => Photos.FirstOrDefault(x => x.Name == name);

        public void RemovePhoto(string name)
        {
            var photo = GetPhoto(name);
            if(photo.HasNoValue)
            {
                return;
            }
            _photos.Remove(photo.Value);
        }

        public void AddTag(string tag)
        {
            _tags.Add(tag);
        }

        public void RemoveTag(string tag)
        {
            _tags.Remove(tag);
        }

        public void SetDescription(string description)
        {
            if (description.Empty())
            {
                Description = string.Empty;

                return;
            }
            if (description.Length > 500)
            {
                throw new DomainException(OperationCodes.InvalidRemarkDescription, 
                    "Remark Description is too long.");
            }
            if (Description.EqualsCaseInvariant(description))
            {
                return;
            }
            Description = description;
        }

        public void VotePositive(string userId, DateTime createdAt)
        {
            if(Votes.Any(x => x.UserId == userId && x.Positive))
            {
                throw new DomainException(OperationCodes.CannotSubmitVote,
                    $"User with id: '{userId}' has already " + 
                    $"submitted a positive vote for remark with id: '{Id}'.");
            }
            var negativeVote = Votes.SingleOrDefault(x => x.UserId == userId && !x.Positive);
            if (negativeVote != null)
            {
                _votes.Remove(negativeVote);
                Rating++;
            }

            _votes.Add(Vote.GetPositive(userId, createdAt));
            Rating++;
        }

        public void VoteNegative(string userId, DateTime createdAt)
        {
            if(Votes.Any(x => x.UserId == userId && !x.Positive))
            {
                throw new DomainException(OperationCodes.CannotSubmitVote,
                    $"User with id: '{userId}' has already " + 
                    $"submitted a negative vote for remark with id: '{Id}'.");
            }
            var positiveVote = Votes.SingleOrDefault(x => x.UserId == userId && x.Positive);
            if (positiveVote != null)
            {
                _votes.Remove(positiveVote);
                Rating--;
            }

            _votes.Add(Vote.GetNegative(userId, createdAt));
            Rating--;
        }

        public void DeleteVote(string userId)
        {
            var vote = Votes.SingleOrDefault(x => x.UserId == userId);
            if (vote == null)
            {
                throw new DomainException(OperationCodes.CannotDeleteVote, 
                    $"User with id: '{userId}' has not " + 
                    $"submitted any vote for remark with id: '{Id}'.");              
            }
            if (vote.Positive)
            {
                Rating--;
            }
            else
            {
                Rating++;
            }
            _votes.Remove(vote);
        }

        public void SetProcessingState(User user, string description = null)
            => SetState(RemarkState.Processing(RemarkUser.Create(user), description));

        public void SetResolvedState(User user, string description = null, Location location = null)
            => SetState(RemarkState.Resolved(RemarkUser.Create(user), description, location));

        public void SetRenewedState(User user, string description = null)
            => SetState(RemarkState.Renewed(RemarkUser.Create(user), description));

        public void SetCanceledState(User user, string description = null)
            => SetState(RemarkState.Canceled(RemarkUser.Create(user), description));

        public Maybe<RemarkState> GetLatestStateOf(string state) 
            => States.OrderByDescending(x => x.CreatedAt).FirstOrDefault(x => x.State == state); 

        private void SetState(RemarkState state) 
        {
            if(state == null)
            {
                throw new DomainException(OperationCodes.RemarkStateNotProvided, 
                    "Remark state can not be null.");
            }

            var latestState = _states.LastOrDefault();
            if(latestState == null)
            {
                _states.Add(state);
                State = state;

                return;
            }
            if(latestState.State == RemarkState.Names.Canceled)
            {
                throw new DomainException(OperationCodes.CannotSetState,
                    $"Can not set state to '{state}' for remark with id: '{Id}'" +
                     "as it was canceled.");                
            }
            if(latestState.State == state.State &&  latestState.State != RemarkState.Names.Processing)
            {
                throw new DomainException(OperationCodes.CannotSetState,
                    $"Can not set state to '{state}' for remark with id: '{Id}'" +
                     "as it's the same as the previous one.");
            }
            _states.Add(state);
            State = state;
        }
    }
}