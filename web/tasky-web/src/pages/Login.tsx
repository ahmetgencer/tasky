import type { FormEvent } from 'react';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { api } from '../lib/axios';
import { useAuthStore } from '../store/auth';

export default function Login() {
  const [email, setEmail] = useState('demo@tasky.local');
  const [password, setPassword] = useState('P@ssw0rd!');
  const [error, setError] = useState<string | null>(null);
  const setToken = useAuthStore((s) => s.setToken);
  const nav = useNavigate();

  async function handleLogin(e: FormEvent) {
    e.preventDefault();
    setError(null);
    try {
      const res = await api.post<{ token: string }>('/api/auth/login', {
        email,
        password,
      });
      setToken(res.data.token);
      nav('/projects');
    } catch (err: any) {
      setError(err?.response?.data ?? 'Login failed');
    }
  }

  return (
    <div className='min-h-full flex items-center justify-center p-6'>
      <form
        onSubmit={handleLogin}
        className='w-full max-w-sm space-y-3 bg-white p-6 rounded-2xl shadow'
      >
        <h1 className='text-xl font-semibold'>Sign in</h1>
        <input
          className='w-full border p-2 rounded'
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder='Email'
        />
        <input
          className='w-full border p-2 rounded'
          type='password'
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder='Password'
        />
        {error && <div className='text-red-600 text-sm'>{String(error)}</div>}
        <button className='w-full bg-black text-white py-2 rounded hover:opacity-90'>
          Login
        </button>
      </form>
    </div>
  );
}
