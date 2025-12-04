import { ChevronDown } from 'lucide-react';
import { cn } from '../../utils/helpers';

const Select = ({ label, options, error, className, ...props }) => (
  <div className="space-y-1.5 w-full text-left">
    {label && <label className="text-sm font-medium text-slate-300 ml-1">{label}</label>}
    <div className="relative">
      <select
        className={cn(
          "w-full bg-black/20 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:ring-2 focus:ring-indigo-500/50 focus:border-indigo-500/50 transition-all backdrop-blur-sm appearance-none cursor-pointer",
          error && "border-rose-500/50",
          className
        )}
        {...props}
      >
        {options.map(opt => (
          <option key={opt.value} value={opt.value} className="bg-slate-800 text-white">
            {opt.label}
          </option>
        ))}
      </select>
      <ChevronDown className="absolute right-4 top-1/2 -translate-y-1/2 text-slate-400 pointer-events-none" size={16} />
    </div>
  </div>
);

export default Select;