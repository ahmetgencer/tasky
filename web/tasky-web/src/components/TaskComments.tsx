// tasky-web/src/components/TaskComments.tsx
import { useEffect, useState } from 'react';
import { api } from '../lib/axios';
import type { CommentDto } from '../types';

export default function TaskComments({ taskId }: { taskId: string }) {
  const [items, setItems] = useState<CommentDto[]>([]);
  const [text, setText] = useState('');
  const [loading, setLoading] = useState(false);

  async function load() {
    const res = await api.get<CommentDto[]>(`/api/tasks/${taskId}/comments`);
    setItems(res.data);
  }

  async function add() {
    if (!text.trim()) return;
    setLoading(true);
    try {
      await api.post(`/api/tasks/${taskId}/comments`, { content: text.trim() });
      setText('');
      await load();
    } finally {
      setLoading(false);
    }
  }

  async function remove(id: string) {
    await api.delete(`/api/comments/${id}`);
    await load();
  }

  useEffect(() => {
    load();
  }, [taskId]);

  return (
    <div className='mt-3 border-t pt-3 space-y-3'>
      <div className='flex gap-2'>
        <input
          className='border rounded p-2 flex-1'
          placeholder='Write a comment…'
          value={text}
          onChange={(e) => setText(e.target.value)}
          disabled={loading}
        />
        <button
          onClick={add}
          disabled={loading}
          className='px-3 rounded bg-black text-white'
        >
          Add
        </button>
      </div>

      <ul className='space-y-2'>
        {items.map((c) => (
          <li
            key={c.id}
            className='text-sm bg-gray-50 border rounded p-2 flex justify-between items-center'
          >
            <div>
              <div className='font-medium'>
                {new Date(c.createdAt).toLocaleString()}
              </div>
              <div>{c.content}</div>
            </div>
            <button
              onClick={() => remove(c.id)}
              className='text-red-600 text-xs'
            >
              Delete
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}
