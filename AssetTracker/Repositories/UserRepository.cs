using System;
using AssetTracker.Models;


namespace AssetTracker.Repositories
{
	public class UserRepository:IUserRepository
	{
        private readonly ICollection<User> users;
        //private readonly ApplicationDbContext _context;
		public UserRepository()
		{
            users = new List<User>();
            //_context = context;
		}

        public async Task AddUserAsync(User user)
        {
            users.Add(user);
            await Task.CompletedTask;
            //await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            var user = users.FirstOrDefault(p => p.UserId == userId);

            if (user == null)
            {
                // Handle null case here
                throw new InvalidOperationException("User not found.");
            }
            else return user;
            //return await users
            //            .Include(u => u.Portfolios)
            //                .ThenInclude(p => p.Positions)
            //            .Include(u => u.Watchlists)
            //                .ThenInclude(w => w.Stocks)
            //            .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await Task.FromResult(users);
            //return await _context.Users.ToListAsync();
        }

        public async Task RemoveUserAsync(Guid userId)
        {

            var user = users.FirstOrDefault(p => p.UserId == userId);
            if (user != null)
            {
                users.Remove(user); // Remove position from the in-memory list (replace with DB delete logic)
            }
            else
            {
               throw new InvalidOperationException("User not found."); ;

            }
            await Task.CompletedTask; // S
        }

        public async Task UpdateUserAsync(User user)
        {
            var existingUser = users.FirstOrDefault(p => p.UserId == user.UserId);
            if (existingUser != null)
            {
                // Update the portfolio's properties (you could add more business logic here)
                existingUser = user;
            }
            else
            {
                throw new InvalidOperationException("User not found."); ;

            }
            await Task.CompletedTask; // Simulate async task
            //_context.Users.Update(user);
            //await _context.SaveChangesAsync();
        }
    }
}

