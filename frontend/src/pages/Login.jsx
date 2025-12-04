import { useState } from 'react';
import { LayoutDashboard, AlertCircle } from 'lucide-react';
import { GlassCard, Button, Input } from '../components/ui';
import { authService } from '../services/api';

const Login = ({ onLogin }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsLoading(true);
    setError('');

    try {
      const data = await authService.login(
        e.target.email.value,
        e.target.password.value
      );
      onLogin(data);
    } catch (err) {
      setError('Invalid credentials');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen w-full flex items-center justify-center p-4">
      <GlassCard className="w-full max-w-md p-8 relative overflow-hidden bg-black/40 border-white/10">
        {/* Top gradient bar */}
        <div className="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-indigo-500 via-purple-500 to-pink-500" />

        {/* Header */}
        <div className="text-center mb-8">
          <div className="h-16 w-16 mx-auto bg-gradient-to-tr from-indigo-500 to-purple-500 rounded-2xl flex items-center justify-center text-white shadow-xl shadow-indigo-500/20 mb-4">
            <LayoutDashboard size={32} />
          </div>
          <h1 className="text-3xl font-bold text-white mb-2">Welcome Back</h1>
          <p className="text-slate-400">Sign in to manage your tasks</p>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="space-y-5">
          <Input
            name="email"
            type="email"
            placeholder="name@company.com"
            required
          />
          <Input
            name="password"
            type="password"
            placeholder="••••••••"
            required
          />

          {error && (
            <div className="p-3 rounded-xl bg-rose-500/10 border border-rose-500/20 text-rose-300 text-sm flex items-center gap-2">
              <AlertCircle size={16} /> {error}
            </div>
          )}

          <Button type="submit" className="w-full h-12 text-lg" isLoading={isLoading}>
            Sign In
          </Button>
        </form>
      </GlassCard>
    </div>
  );
};

export default Login;