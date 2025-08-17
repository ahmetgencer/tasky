import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../lib/axios';
import { useAuthStore } from '../store/auth';
import type { Project } from '../types';

export default function Projects() {
  const [items, setItems] = useState<Project[]>([]);
  const [name, setName] = useState('');
  const [me, setMe] = useState<{ id: string; email?: string } | null>(null);
  const logout = useAuthStore((s) => s.logout);

  async function load() {
    const [projectsRes, meRes] = await Promise.all([
      api.get<Project[]>('/api/projects'),
      api.get<{ id: string; email?: string }>('/api/auth/me'),
    ]);
    setItems(projectsRes.data);
    setMe(meRes.data);
  }

  async function create() {
    if (!name) return;
    await api.post<{ id: string }>('/api/projects', { name });
    setName('');
    await load();
  }

  useEffect(() => {
    load();
  }, []);

  return (
    <div className='max-w-3xl mx-auto p-6 space-y-6'>
      <header className='flex items-center justify-between'>
        <h1 className='text-2xl font-bold'>Projects</h1>
        <div className='flex items-center gap-3 text-sm'>
          {me?.email && (
            <span className='text-gray-600'>
              Signed in as <b>{me.email}</b>
            </span>
          )}
          <button onClick={logout} className='underline'>
            Logout
          </button>
        </div>
      </header>

      <div className='flex gap-2'>
        <input
          className='border p-2 rounded flex-1'
          placeholder='New project name'
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
        <button onClick={create} className='bg-black text-white rounded px-4'>
          Add
        </button>
      </div>

      <ul className='space-y-2'>
        {items.map((p) => (
          <li key={p.id} className='border rounded p-3 hover:bg-gray-50'>
            <Link to={`/projects/${p.id}`} className='font-medium'>
              {p.name}
            </Link>
            <div className='text-xs text-gray-500'>Owner: {p.ownerId}</div>
          </li>
        ))}
      </ul>
    </div>
  );
}
