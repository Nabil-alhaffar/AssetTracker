using System;
using AssetTracker.Models;


namespace AssetTracker.Repositories
{
	public class UserRepository:IUserRepository
	{
        private readonly ICollection<User> users = new List<User>();
        //private readonly ApplicationDbContext _context;
		public UserRepository()
		{
            //_context = context;
		}

        public async Task AddUserAsync(User user)
        {
            users.Add(user);
            await Task.CompletedTask;
            //await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserAsync(int userId)
        {
            var user = users.FirstOrDefault(p => p.UserId == userId);
            if (user != null)
                return await Task.FromResult(user);
            else return null;
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

        public async Task RemoveUserAsync(int userId)
        {

            var user = users.FirstOrDefault(p => p.UserId == userId);
            if (user != null)
            {
                users.Remove(user); // Remove position from the in-memory list (replace with DB delete logic)
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
            await Task.CompletedTask; // Simulate async task
            //_context.Users.Update(user);
            //await _context.SaveChangesAsync();
        }
    }
}

