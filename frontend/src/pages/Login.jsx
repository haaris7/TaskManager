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
      <GlassCard className="w-full max-w-md p-8 relative overflow-hidden bg-white/80 border-white shadow-2xl">
        {/* Top gradient bar */}
        <div className="absolute top-0 left-0 w-full h-1.5 bg-gradient-to-r from-sky-400 via-teal-400 to-emerald-400" />

        {/* Header */}
        <div className="text-center mb-10 mt-4">
          <div className="h-20 w-20 mx-auto bg-gradient-to-tr from-sky-400 to-teal-500 rounded-2xl flex items-center justify-center text-white shadow-xl shadow-sky-500/30 mb-6 rotate-3 hover:rotate-6 transition-transform">
            <LayoutDashboard size={40} />
          </div>
          <h1 className="text-3xl font-bold text-slate-800 mb-2">Welcome Back</h1>
          <p className="text-slate-500 font-medium">TaskFlow Corporate Management</p>
        </div>

        {/* Form */}
        <form onSubmit={handleSubmit} className="space-y-6">
          <Input
            name="email"
            type="email"
            placeholder="name@company.com"
            label="Email Address"
            required
            className="bg-slate-50 border-slate-200 focus:bg-white"
          />
          <Input
            name="password"
            type="password"
            placeholder="••••••••"
            label="Password"
            required
            className="bg-slate-50 border-slate-200 focus:bg-white"
          />

          {error && (
            <div className="p-4 rounded-xl bg-rose-50 border border-rose-100 text-rose-600 text-sm flex items-center gap-2 font-medium">
              <AlertCircle size={18} /> {error}
            </div>
          )}

          <Button type="submit" className="w-full h-12 text-lg shadow-lg shadow-sky-500/20" isLoading={isLoading}>
            Sign In
          </Button>
        </form>
      </GlassCard>
    </div>
  );
};

export default Login;