import { useEffect, useMemo, useState } from 'react';
import { useParams } from 'react-router-dom';
import {
  DragDropContext,
  Droppable,
  Draggable,
  type DropResult,
} from '@hello-pangea/dnd';
import { api } from '../lib/axios';
import TaskComments from '../components/TaskComments';
import {
  TaskyStatus,
  TaskyStatusLabel,
  type TaskyStatus as TaskyStatusType,
  type TaskDto,
} from '../types';

const ORDER: TaskyStatusType[] = [
  TaskyStatus.Todo,
  TaskyStatus.InProgress,
  TaskyStatus.Done,
];

export default function ProjectBoard() {
  const { id: projectId } = useParams();
  const [tasks, setTasks] = useState<TaskDto[]>([]);
  const [title, setTitle] = useState('');
  const [openComments, setOpenComments] = useState<Record<string, boolean>>({});

  const [columns, setColumns] = useState<Record<TaskyStatusType, TaskDto[]>>({
    [TaskyStatus.Todo]: [],
    [TaskyStatus.InProgress]: [],
    [TaskyStatus.Done]: [],
  });

  async function load() {
    const res = await api.get<TaskDto[]>('/api/tasks', {
      params: { projectId },
    });
    setTasks(res.data);
  }

  async function addTask() {
    if (!title.trim()) return;
    await api.post('/api/tasks', {
      projectId,
      title: title.trim(),
      status: TaskyStatus.Todo,
    });
    setTitle('');
    await load();
  }

  async function updateStatus(task: TaskDto, status: TaskyStatusType) {
    await api.put(`/api/tasks/${task.id}`, {
      id: task.id,
      title: task.title,
      description: task.description ?? null,
      status,
      priority: task.priority,
      assigneeId: task.assigneeId ?? null,
      dueDate: task.dueDate ?? null,
    });
  }

  async function removeTask(id: string) {
    await api.delete(`/api/tasks/${id}`);
    await load();
  }

  useEffect(() => {
    const next: Record<TaskyStatusType, TaskDto[]> = {
      [TaskyStatus.Todo]: [],
      [TaskyStatus.InProgress]: [],
      [TaskyStatus.Done]: [],
    };
    for (const t of tasks) next[t.status].push(t);
    setColumns(next);
  }, [tasks]);

  useEffect(() => {
    if (projectId) load();
  }, [projectId]);

  function StatusBadge({ s }: { s: TaskyStatusType }) {
    const colorBy: Record<TaskyStatusType, string> = {
      [TaskyStatus.Todo]: 'bg-gray-200',
      [TaskyStatus.InProgress]: 'bg-yellow-200',
      [TaskyStatus.Done]: 'bg-green-200',
    };
    return (
      <span className={`text-xs px-2 py-0.5 rounded ${colorBy[s]}`}>
        {TaskyStatusLabel[s]}
      </span>
    );
  }

  function onDragEnd(result: DropResult) {
    const { source, destination, draggableId } = result;
    if (!destination) return;

    const from = Number(source.droppableId) as TaskyStatusType;
    const to = Number(destination.droppableId) as TaskyStatusType;
    if (from === to && source.index === destination.index) return;

    setColumns((prev) => {
      const copy: Record<TaskyStatusType, TaskDto[]> = {
        [TaskyStatus.Todo]: [...prev[TaskyStatus.Todo]],
        [TaskyStatus.InProgress]: [...prev[TaskyStatus.InProgress]],
        [TaskyStatus.Done]: [...prev[TaskyStatus.Done]],
      };

      const [moved] = copy[from].splice(source.index, 1);
      const task = moved ?? tasks.find((t) => t.id === draggableId);
      if (!task) return prev;

      copy[to].splice(destination.index, 0, { ...task, status: to });

      if (from !== to) {
        updateStatus(task, to).then(load).catch(console.error);
      }
      return copy;
    });
  }

  const counts = useMemo(
    () => ({
      Todo: columns[TaskyStatus.Todo].length,
      InProgress: columns[TaskyStatus.InProgress].length,
      Done: columns[TaskyStatus.Done].length,
    }),
    [columns]
  );

  return (
    <div className='p-6 space-y-6'>
      <div className='max-w-3xl mx-auto flex gap-2'>
        <input
          className='border p-2 rounded flex-1'
          placeholder='New task title'
          value={title}
          onChange={(e) => setTitle(e.target.value)}
        />
        <button onClick={addTask} className='bg-black text-white rounded px-4'>
          Add
        </button>
      </div>

      <DragDropContext onDragEnd={onDragEnd}>
        <div className='grid md:grid-cols-3 gap-4'>
          {ORDER.map((col) => (
            <Droppable droppableId={String(col)} key={col}>
              {(provided, snapshot) => (
                <div
                  ref={provided.innerRef}
                  {...provided.droppableProps}
                  className={`border rounded p-3 min-h-[200px] transition-colors ${
                    snapshot.isDraggingOver ? 'bg-gray-50' : 'bg-white'
                  }`}
                >
                  <h2 className='font-semibold mb-3'>
                    {col === TaskyStatus.Todo
                      ? 'Todo'
                      : col === TaskyStatus.InProgress
                      ? 'In Progress'
                      : 'Done'}
                    <span className='ml-2 text-xs text-gray-500'>
                      (
                      {col === TaskyStatus.Todo
                        ? counts.Todo
                        : col === TaskyStatus.InProgress
                        ? counts.InProgress
                        : counts.Done}
                      )
                    </span>
                  </h2>

                  <div className='space-y-2'>
                    {columns[col].map((t, i) => (
                      <Draggable draggableId={t.id} index={i} key={t.id}>
                        {(dragProvided, dragSnapshot) => (
                          <div
                            ref={dragProvided.innerRef}
                            {...dragProvided.draggableProps}
                            {...dragProvided.dragHandleProps}
                            className={`rounded border p-3 bg-white shadow-sm ${
                              dragSnapshot.isDragging
                                ? 'ring-2 ring-black/20'
                                : ''
                            }`}
                          >
                            <div className='flex items-center justify-between'>
                              <span className='font-medium'>{t.title}</span>
                              <StatusBadge s={t.status} />
                            </div>

                            {t.description && (
                              <p className='text-sm text-gray-600 mt-1'>
                                {t.description}
                              </p>
                            )}

                            <div className='mt-2 flex flex-wrap items-center gap-2'>
                              <select
                                className='border rounded p-1 text-sm'
                                value={t.status}
                                onChange={(e) =>
                                  updateStatus(
                                    t,
                                    Number(e.target.value) as TaskyStatusType
                                  ).then(load)
                                }
                              >
                                <option value={TaskyStatus.Todo}>Todo</option>
                                <option value={TaskyStatus.InProgress}>
                                  In Progress
                                </option>
                                <option value={TaskyStatus.Done}>Done</option>
                              </select>

                              <button
                                onClick={() =>
                                  setOpenComments((s) => ({
                                    ...s,
                                    [t.id]: !s[t.id],
                                  }))
                                }
                                className='text-sm text-blue-600'
                              >
                                {openComments[t.id]
                                  ? 'Hide comments'
                                  : 'Comments'}
                              </button>

                              <button
                                onClick={() => removeTask(t.id)}
                                className='text-sm text-red-600'
                              >
                                Delete
                              </button>
                            </div>

                            {openComments[t.id] && (
                              <TaskComments taskId={t.id} />
                            )}
                          </div>
                        )}
                      </Draggable>
                    ))}
                    {provided.placeholder}
                  </div>
                </div>
              )}
            </Droppable>
          ))}
        </div>
      </DragDropContext>
    </div>
  );
}
